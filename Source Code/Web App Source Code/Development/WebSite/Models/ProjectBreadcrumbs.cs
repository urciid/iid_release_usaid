using System;
using System.Data;
using System.Linq;

using IID.BusinessLayer.Domain;
using System.Collections.Generic;

namespace IID.WebSite.Models
{
    public class ProjectBreadcrumbs : Base
    {
        #region Static Methods

        public static ProjectBreadcrumbs ForActivity(int activityId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.Activity);
            bc.SetActivity(activityId);
            return bc;
        }

        public static ProjectBreadcrumbs ForIndicator(int indicatorId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.Indicator);
            bc.SetIndicator(indicatorId);
            return bc;
        }

        public static ProjectBreadcrumbs ForSite(int siteId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.Site);
            bc.SetSite(siteId);
            return bc;
        }

        public static ProjectBreadcrumbs ForObservationView(int indicatorId, int siteId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.ObservationView);
            bc.SetIndicator(indicatorId);
            bc.SetSite(siteId);
            return bc;
        }

        public static ProjectBreadcrumbs ForObservationRecord(int indicatorId, int siteId, DateTime beginDate, DateTime endDate)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.ObservationRecord);
            bc.SetIndicator(indicatorId);
            bc.SetSite(siteId);
            bc.ObservationBeginDate = beginDate;
            bc.ObservationEndDate = endDate;
            return bc;
        }

        public static ProjectBreadcrumbs ForCoachReport(int activityId, int siteId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.CoachReport);
            bc.SetActivity(activityId);
            bc.SetSite(siteId);
            return bc;
        }

        public static ProjectBreadcrumbs ForRankByImprovement(int activityId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.RankByImprovement);
            bc.SetActivity(activityId);
            return bc;
        }

        public static ProjectBreadcrumbs ForChart(int indicatorId)
        {
            var bc = new ProjectBreadcrumbs(ProjectBreadcrumbsType.Chart);
            bc.SetIndicator(indicatorId);
            return bc;
        }

        #endregion

        #region Instance Methods

        private ProjectBreadcrumbs(ProjectBreadcrumbsType type)
        {
            Type = type;
        }

        private void SetActivity(int activityId)
        {
            SetActivity(Context.t_activity.Find(activityId));
        }

        private void SetActivity(t_activity activity)
        {
            ActivityId = activity.activity_id;
            ActivityName = activity.get_name_translated(CurrentLanguageId);
        }

        private void SetIndicator(int indicatorId)
        {
            t_indicator indicator = Context.t_indicator.Find(indicatorId);
            Indicators = UserAssignedObjects.GetIndicators(indicator.aim.activity.aims.SelectMany(a => a.indicators), CurrentUser).Select(i => new Indicator(i, false, true, true)).ToList();
            SelectedIndicatorId = indicatorId;

            SetActivity(indicator.aim.activity);
            // TODO: Recurse to project, organization?
        }

        private void SetSite(int siteId)
        {
            IEnumerable<t_site> sites;
            if (ActivityId.HasValue)
            {
                // Get sites user is allowed to access (view or update).
                DataTable userSites = StoredProcedures.GetUserSites(CurrentUserId, ActivityId.Value);
                var userSiteIds = userSites.AsEnumerable().Select(r => (int)r["site_id"]).ToArray();
                sites = Context.t_site.Where(e => userSiteIds.Contains(e.site_id));
            }
            else
            {
                sites = new t_site[] { Context.t_site.Find(siteId) };
            }
            Sites = sites.OrderBy(e => e.name).ToDictionary(k => k.site_id, v => v.name);
            SelectedSiteId = siteId;
        }

        #endregion

        #region Properties

        public int? ActivityId { get; private set; }
        public string ActivityName { get; private set; }

        public int? SelectedIndicatorId { get; private set; }
        public string SelectedIndicatorName
        {
            get { return SelectedIndicatorId.HasValue ? Indicators.First(i => i.IndicatorId == SelectedIndicatorId.Value).Name : null; }
        }
        public List<Indicator> Indicators { get; set; }

        public int? SelectedSiteId { get; private set; }
        public string SelectedSiteName
        {
            get { return SelectedSiteId.HasValue ? Sites[SelectedSiteId.Value] : null; }
        }
        public Dictionary<int, string> Sites { get; private set; }

        public DateTime? ObservationBeginDate { get; private set; }
        public DateTime? ObservationEndDate { get; private set; }

        public ProjectBreadcrumbsType Type { get; private set; }

        #endregion
    }

    public enum ProjectBreadcrumbsType
    {
        Activity,
        Indicator,
        Site,
        ObservationView,
        ObservationRecord,
        CoachReport,
        RankByImprovement,
        Chart
    }
}