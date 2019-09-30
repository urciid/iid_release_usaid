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
    public class UserStatistics
    {
        public UserStatistics(int userId, string request = "All")
        {
            using (Entity context = new Entity())
            {
                UserStats = new List<UserStatisticDetails>();               

                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_user_statistics";
                cmd.Parameters.Add(new SqlParameter("@user_id", userId));
                cmd.Parameters.Add(new SqlParameter("@request", request));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    // User statistics
                    var user_stats = ((IObjectContextAdapter)context).ObjectContext.Translate<UserStatisticDetails>(reader);
                    foreach (var item in user_stats)
                        UserStats.Add(item);                    
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }
        public ICollection<UserStatisticDetails> UserStats { get; set; }
    }

    public sealed class UserStatisticDetails
    {

        private string iid_table { get; set; }
        public string TableName { get { return iid_table; } }

        private int user_id { get; set; }
        public int UserID { get { return user_id; } }

        private string first_name { get; set; }
        public string FirstName { get { return first_name; } }

        private string last_name { get; set; }
        public string LastName { get { return last_name; } }

        private int wtd { get; set; }
        public int WTDCount { get { return wtd; } }

        private int mtd { get; set; }
        public int MTDCount { get { return mtd; } }

        private int ytd { get; set; }
        public int YTDCount { get { return ytd; } }

        private int alltime { get; set;}
        public int AllTimeCount { get { return alltime; } }
        
    }
    
}