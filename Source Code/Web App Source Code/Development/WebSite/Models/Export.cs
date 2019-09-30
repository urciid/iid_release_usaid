using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace IID.WebSite.Models
{
    public class Export
    {
        SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Identity"].ConnectionString);

        public DataSet GetObservationData(int UserID, int ActivityID, int IndicatorID, int SiteID)
        {
            SqlCommand cmd = new SqlCommand("p_export_observations", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@activity_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@indicator_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@site_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@user_id", System.Data.SqlDbType.Int));
            cmd.Parameters["@activity_id"].Value = ActivityID;
            cmd.Parameters["@indicator_id"].Value = IndicatorID;
            cmd.Parameters["@site_id"].Value = SiteID;
            cmd.Parameters["@user_id"].Value = UserID;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            conn.Close();

            return ds;
        }

        public DataSet GetObservationDataResearch(int UserID, int ActivityID, int IndicatorID = 0, int SiteID = 0)
        {
            SqlCommand cmd = new SqlCommand("p_export_observations_for_research", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@activity_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@indicator_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@site_id", System.Data.SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@user_id", System.Data.SqlDbType.Int));
            cmd.Parameters["@activity_id"].Value = ActivityID;
            cmd.Parameters["@indicator_id"].Value = IndicatorID;
            cmd.Parameters["@site_id"].Value = SiteID;
            cmd.Parameters["@user_id"].Value = UserID;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            conn.Close();

            return ds;
        }

    }
}