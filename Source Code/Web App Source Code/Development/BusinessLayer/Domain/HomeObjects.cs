using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace IID.BusinessLayer.Domain
{
    public sealed class HomeObjects
    {
        private HomeObjects() { }

        public HomeObjects(int userId, int languageId)
        {
            using (Entity context = new Entity())
            {
                AssignedActivities = new List<AssignedActivity>();
                AssignedSites = new List<AssignedSite>();
                FavoriteActivities = new List<FavoriteActivity>();
                FavoriteSites = new List<FavoriteSite>();
                FavoriteCharts = new List<FavoriteChart>();
                AssignedCountries = new List<AssignedCountry>();

                context.Database.Initialize(false);

                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.p_get_home_objects";
                cmd.Parameters.Add(new SqlParameter("@user_id", userId));
                cmd.Parameters.Add(new SqlParameter("@language_id", languageId));

                try
                {
                    context.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    // Assigned activities
                    var assignedActivities = ((IObjectContextAdapter)context).ObjectContext.Translate<AssignedActivity>(reader);
                    foreach (var item in assignedActivities)
                        AssignedActivities.Add(item);

                    // Assigned sites
                    reader.NextResult();
                    var assignedSites = ((IObjectContextAdapter)context).ObjectContext.Translate<AssignedSite>(reader);
                    foreach (var item in assignedSites)
                        AssignedSites.Add(item);

                    // Favorite activities
                    reader.NextResult();
                    var favoriteActivities = ((IObjectContextAdapter)context).ObjectContext.Translate<FavoriteActivity>(reader);
                    foreach (var item in favoriteActivities)
                        FavoriteActivities.Add(item);

                    // Favorite sites
                    reader.NextResult();
                    var favoriteSites = ((IObjectContextAdapter)context).ObjectContext.Translate<FavoriteSite>(reader);
                    foreach (var item in favoriteSites)
                        FavoriteSites.Add(item);

                    // Favorite charts
                    reader.NextResult();
                    var favoriteCharts = ((IObjectContextAdapter)context).ObjectContext.Translate<FavoriteChart>(reader);
                    foreach (var item in favoriteCharts)
                        FavoriteCharts.Add(item);

                    // Assigned countries
                    reader.NextResult();
                    var assignedCountries = ((IObjectContextAdapter)context).ObjectContext.Translate<AssignedCountry>(reader);
                    foreach (var item in assignedCountries)
                        AssignedCountries.Add(item);
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }

        public ICollection<AssignedCountry> AssignedCountries { get; set; }
        public ICollection<AssignedActivity> AssignedActivities { get; set; }
        public ICollection<AssignedSite> AssignedSites { get; set; }
        public ICollection<FavoriteActivity> FavoriteActivities { get; set; }        
        public ICollection<FavoriteSite> FavoriteSites { get; set; }
        public ICollection<FavoriteChart> FavoriteCharts { get; set; }
    }

    public sealed class AssignedActivity
    {
        private int activity_id { get; set; }
        public int ActivityId {  get { return activity_id; } }

        private string activity_name { get; set; }
        public string ActivityName { get { return activity_name; } }

        private int project_id { get; set; }
        public int ProjectId { get { return project_id; } }

        private string project_name { get; set; }
        public string ProjectName { get { return project_name; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }

        private bool update_access { get; set; }
        public bool CanUpdate { get { return update_access; } }

        private bool has_active_indicators { get; set; }
        public bool HasActiveIndicators { get { return has_active_indicators; } }
    }

    public sealed class AssignedSite
    {
        public string Key {  get { return String.Concat(SiteId, "_", IndicatorId); } }

        private int site_id { get; set; }
        public int SiteId { get { return site_id; } }

        private string site_name { get; set; }
        public string SiteName { get { return site_name; } }

        private int activity_id { get; set; }
        public int ActivityId { get { return activity_id; } }

        private string activity_name { get; set; }
        public string ActivityName { get { return activity_name; } }

        private int indicator_id { get; set; }
        public int IndicatorId { get { return indicator_id; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }

        private bool update_access { get; set; }
        public bool CanUpdate { get { return update_access; } }
    }

    public sealed class FavoriteActivity
    {
        private int activity_id { get; set; }
        public int ActivityId { get { return activity_id; } }

        private string activity_name { get; set; }
        public string ActivityName { get { return activity_name; } }

        private int project_id { get; set; }
        public int ProjectId { get { return project_id; } }

        private string project_name { get; set; }
        public string ProjectName { get { return project_name; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }

        private bool has_active_indicators { get; set; }
        public bool HasActiveIndicators { get { return has_active_indicators; } }
    }

    public sealed class AssignedCountry
    {
        private int country_id { get; set; }
        public int CountryId { get { return country_id; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }

        private int observations_last7days { get; set; }
        public int ObservationsLast7Days { get { return observations_last7days; } }

        private int observations_last30days { get; set; }
        public int ObservationsLast30Days { get { return observations_last30days; } }

        private int observations_total { get; set; }
        public int ObservationsTotal { get { return observations_total; } }
    }

    public sealed class FavoriteSite
    {
        private int site_id { get; set; }
        public int SiteId { get { return site_id; } }

        private string site_name { get; set; }
        public string SiteName { get { return site_name; } }

        private string country_name { get; set; }
        public string CountryName { get { return country_name; } }
    }

    public sealed class FavoriteChart
    {
        private int user_id { get; set; }
        public int UserId { get { return user_id; } }

        private string chart_name { get; set; }
        public string ChartName { get { return chart_name; } }

        private DateTime updated_date { get; set; }
        public DateTime UpdatedDate { get { return updated_date; } }
    }
}
