using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace IID.BusinessLayer.Domain
{
    public sealed class FieldIdParentTypes
    {
        public const string Funder = "fun";
        public const string FundingType = "fnt";
        public const string IndicatorCollectionFrequency = "frq";
        public const string IndicatorGroup = "igr";
        public const string IndicatorType = "ind";
        public const string PopulationDensity = "rou";
        public const string QIIndexScore = "qi";
        public const string RatioPer = "ratio";
        public const string RequestType = "req";
        public const string Sampling = "sam";
        public const string TechnicalArea = "tec";
        public const string TechnicalAreaSubtype = "tas";
        public const string UserRole = "rol";
        public const string Wave = "wave";
        public const string ReportClass = "rpc";
    }

    public enum IndicatorType { Average, Count, Percentage, Ratio, YesNo }

    public enum DataCollectionFrequency { Daily, Weekly, BiWeekly, Monthly, Quarterly }

    public enum Geography {  Country, AdministrativeDivision, Site }

    public static class StoredProcedures
    {
        public static DataTable GetObservations(int indicatorId, int siteId, DateTime? observationDate)
        {
            return ExecuteStoredProcedureDataSet(
                "dbo.p_get_observations",
                new SqlParameter("@indicator_id", indicatorId),
                new SqlParameter("@site_id", siteId),
                new SqlParameter("@observation_date", observationDate)).Tables[0];
        }

        public static DataTable GetUserSites(int userId, int? activityId)
        {
            return ExecuteStoredProcedureDataSet(
                "dbo.p_get_user_sites",
                new SqlParameter("@user_id", userId),
                new SqlParameter("@activity_id", activityId)).Tables[0];
        }

        private static DataSet ExecuteStoredProcedureDataSet(string storedProcedureName, params SqlParameter[] parameters)
        {
            DataSet result = new DataSet();

            using (Entity context = new Entity())
            {
                context.Database.Initialize(false);

                DbCommand cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;
                foreach (SqlParameter parameter in parameters)
                    cmd.Parameters.Add(parameter);

                try
                {
                    context.Database.Connection.Open();
                    DbDataReader reader = cmd.ExecuteReader();

                    do
                    {
                        DataTable tb = new DataTable();
                        tb.Load(reader);
                        result.Tables.Add(tb);

                    } while (!reader.IsClosed);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            return result;
        }

        public static ICollection<T> GetEntities<T>(string sprocName, Dictionary<string, object> parameters)
        {
            List<T> list = new List<T>();

            using (Entity context = new Entity())
            {
                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sprocName;
                foreach (var kvp in parameters)
                    cmd.Parameters.Add(new SqlParameter("@" + kvp.Key, kvp.Value));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    var items = ((IObjectContextAdapter)context).ObjectContext.Translate<T>(reader);
                    foreach (T item in items)
                        list.Add(item);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            return list;
        }
    }

    public static class Globalization
    {
        public static string GetTranslation(string tableName, string columnName, object[] keys, byte languageId)
        {
            // If English, don't try to get a translation.
            if (languageId == (int)IID.BusinessLayer.Globalization.Language.English)
                return null;

            using (Entity context = new Entity())
            {
                string keyValues = String.Join(",", keys);
                t_translation translation = context.t_translation.Find(tableName, columnName, keyValues, languageId);
                return translation?.value;
            }
        }

        public static t_translation SetTranslation(string tableName, string columnName, object[] keys, byte languageId, string value, int userId)
        {
            // If English, don't try to set a translation.
            if (languageId == (int)IID.BusinessLayer.Globalization.Language.English)
                return null;

            using (Entity context = new Entity())
            {
                // Look for an existing translation.
                string keyValues = String.Join(",", keys);
                t_translation translation = context.t_translation.Find(tableName, columnName, keyValues, languageId);

                if (String.IsNullOrEmpty(value))
                {
                    // Don't store a null/empty string. If a previous entry exists, delete it.
                    if (translation != null)
                        context.t_translation.Remove(translation);
                }
                else
                {
                    // Add an entry if necessary, then set the new value.
                    if (translation == null)
                    {
                        translation = new t_translation();
                        translation.table_name = tableName;
                        translation.column_name = columnName;
                        translation.key_values = keyValues;
                        translation.language_id = languageId;
                        translation.createdby_userid = userId;
                        translation.created_date = DateTime.Now;
                        context.t_translation.Add(translation);
                    }
                    else
                    {
                        translation.updatedby_userid = userId;
                        translation.updated_date = DateTime.Now;
                    }
                    translation.value = value;
                }
                context.SaveChanges();

                return translation;
            }
        }
    }
}