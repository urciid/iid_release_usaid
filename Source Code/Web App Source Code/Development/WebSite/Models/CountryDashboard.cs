using System;
using System.Collections.Generic;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace IID.WebSite.Models
{
    public class CountryDashboard
    {
        public CountryDashboard(int userId, int countryID = 0)
        {
            using (Entity context = new Entity())
            {
                CountryStats = new List<CountryDashboardDetails>();

                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_country_statistics";
                cmd.Parameters.Add(new SqlParameter("@user_id", userId));
                cmd.Parameters.Add(new SqlParameter("@country_id", countryID));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    // Country statistics
                    var country_stats = ((IObjectContextAdapter)context).ObjectContext.Translate<CountryDashboardDetails>(reader);
                    foreach (var item in country_stats)
                        CountryStats.Add(item);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }
        public ICollection<CountryDashboardDetails> CountryStats { get; set; }
    }

    public sealed class CountryDashboardDetails
    {

        private string country { get; set; }
        public string CountryName { get { return country; } }

        private string iid_table { get; set; }
        public string TableName { get { return iid_table; } }
        
        private int wtd { get; set; }
        public int WTDCount { get { return wtd; } }

        private int mtd { get; set; }
        public int MTDCount { get { return mtd; } }

        private int ytd { get; set; }
        public int YTDCount { get { return ytd; } }

        private int alltime { get; set; }
        public int AllTimeCount { get { return alltime; } }

    }

}