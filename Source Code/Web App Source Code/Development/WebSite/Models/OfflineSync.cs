using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class OfflineSync
    {
        public OfflineSync()
        {
            _context = new Entity();
            _context.Database.Initialize(false);

            var cmd = _context.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.p_get_offline_objects";
            cmd.Parameters.Add(new SqlParameter("@user_id", Identity.CurrentUser.Id));
            cmd.Parameters.Add(new SqlParameter("@language_id", IidCulture.CurrentLanguageId));

            _context.Database.Connection.Open();
            _reader = cmd.ExecuteReader();

            AssignedActivities = TranslateNextResultSet<AssignedActivity>();
            AssignedSites = TranslateNextResultSet<AssignedSite>();
            var projects = TranslateNextResultSet<t_project>();
            var activities = TranslateNextResultSet<t_activity>();
            var activityAdditionalManagers = TranslateNextResultSet<t_activity_additional_manager>();
            var activityTechnicalAreaSubTypes = TranslateNextResultSet<t_activity_technical_area_subtype>();
            var aims = TranslateNextResultSet<t_aim>();
            var indicators = TranslateNextResultSet<t_indicator>();
            var indicatorAgeRanges = TranslateNextResultSet<t_indicator_age_range>();
            var sites = TranslateNextResultSet<v_site>();
            var observations = TranslateNextResultSet<t_observation>();
            var observationDatePeriods = TranslateNextResultSet<v_observation_date_period>();
            var observationChanges = TranslateNextResultSet<t_observation_change>();
            var observationComments = TranslateNextResultSet<t_observation_comment>();
            var activitySites = TranslateNextResultSet<t_activity_site>();
            var notes = TranslateNextResultSet<t_note>();
            var sexes = TranslateNextResultSet<t_sex>();
            var fieldids = TranslateNextResultSet<t_fieldid>();
            var countries = TranslateNextResultSet<t_country>();
            var users = TranslateNextResultSet<t_user>();

            Projects = projects.Select(e => new Project(e)).ToList();
            Activities = activities.Select(e => new Activity(e)).ToList();
            ActivityAdditionalManagerIds = activityAdditionalManagers.GroupBy(g => g.activity_id).ToDictionary(k => k.Key, v => v.Select(x => x.additional_manager_userid));
            ActivityTechnicalAreaSubTypes = activityTechnicalAreaSubTypes.GroupBy(g => g.activity_id).ToDictionary(k => k.Key, v => v.Select(x => x.technical_area_subtype_fieldid));
            ActivityAimIds = aims.GroupBy(g => g.activity_id).ToDictionary(k => k.Key, v => v.Select(e => e.aim_id));
            ActivitySiteIds = activitySites.GroupBy(g => g.activity_id).ToDictionary(k => k.Key, v => v.Select(e => e.site_id));
            ActivityNoteIds = notes.Where(e => e.activity_id.HasValue).GroupBy(g => g.activity_id.Value).ToDictionary(k => k.Key, v => v.Select(x => x.note_id));
            Aims = aims.Select(e => new Aim(e, false, true)).ToList();
            AimIndicatorIds = indicators.GroupBy(g => g.aim_id).ToDictionary(k => k.Key, v => v.Select(e => e.indicator_id));
            Indicators = indicators.Select(e => new Indicator(e, false, false)).ToList();
            IndicatorAgeRangeIds = indicatorAgeRanges.GroupBy(g => g.indicator_id).ToDictionary(k => k.Key, v => v.Select(x => x.indicator_age_range_id));
            IndicatorAgeRanges = indicatorAgeRanges.Select(e => new Tuple<int, string>(e.indicator_age_range_id, e.age_range)).ToList();
            Sites = sites.Select(e => new Site(e)).ToList();
            SiteActivityIds = activitySites.GroupBy(g => g.site_id).ToDictionary(k => k.Key, v => v.Select(e => e.activity_id));
            SiteNoteIds = notes.Where(e => e.site_id.HasValue).GroupBy(g => g.site_id.Value).ToDictionary(k => k.Key, v => v.Select(x => x.note_id));
            Observations = observations.Select(e => new Observation(e)).ToList();
            ObservationDatePeriods = observationDatePeriods
                .GroupBy(g => new { g.indicator_id, g.site_id })
                .ToDictionary(
                    k => String.Format("{0}_{1}", k.Key.indicator_id, k.Key.site_id),
                    v => v.ToDictionary(
                        l => l.begin_date.ToString("yyyy-MM-dd"),
                        w => new ObservationDatePeriod(w)));
            ObservationIdIndex = observations.GroupBy(g => new { g.indicator_id, g.site_id, g.begin_date }).ToDictionary(k => String.Format("{0}_{1}_{2:yyyyMMdd}", k.Key.indicator_id, k.Key.site_id, k.Key.begin_date), v => v.First().observation_id);
            ObservationChangeIds = observationChanges.GroupBy(g => g.observation_id).ToDictionary(k => k.Key, v => v.Select(e => e.observation_change_id));
            ObservationChanges = observationChanges.Select(e => new ObservationChange(e)).ToList();
            ObservationCommentIds = observationComments.GroupBy(g => g.observation_id).ToDictionary(k => k.Key, v => v.Select(e => e.observation_comment_id));
            ObservationComments = observationComments.Select(e => new ObservationComment(e)).ToList();
            ActivitySites = activitySites.Select(e => new ActivitySite(e)).ToList();
            Notes = notes.Select(e => new Note(e)).ToList();
            Sexes = sexes.OrderBy(e => e.sex_id).ToDictionary(e => e.sex_code, e => e.sex_description);
            FieldIds = fieldids.Select(e => new Tuple<string, string>(e.fieldid.Replace(".", ""), e.value)).ToList();
            Countries = countries.Select(e => new Country(e));
            UserNames = users.Select(e => new Tuple<int, string>(e.user_id, e.full_name)).ToList();

            _reader.Close();
            _context.Database.Connection.Close();
        }

        public string Elapsed { get; set; }

        public IEnumerable<Project> Projects { get; private set; }

        public IEnumerable<Activity> Activities { get; private set; }
        public Dictionary<int, IEnumerable<int>> ActivityAdditionalManagerIds { get; private set; }
        public Dictionary<int, IEnumerable<string>> ActivityTechnicalAreaSubTypes { get; private set; }
        public Dictionary<int, IEnumerable<int>> ActivityAimIds { get; private set; }
        public Dictionary<int, IEnumerable<int>> ActivitySiteIds { get; private set; }
        public Dictionary<int, IEnumerable<int>> ActivityNoteIds { get; private set; }

        public IEnumerable<Aim> Aims { get; private set; }
        public Dictionary<int, IEnumerable<int>> AimIndicatorIds { get; private set; }

        public IEnumerable<Indicator> Indicators { get; private set; }
        public Dictionary<int, IEnumerable<int>> IndicatorAgeRangeIds { get; private set; }
        public List<Tuple<int, string>> IndicatorAgeRanges { get; private set; }

        public IEnumerable<Site> Sites { get; private set; }
        public Dictionary<int, IEnumerable<int>> SiteActivityIds { get; private set; }
        public Dictionary<int, IEnumerable<int>> SiteNoteIds { get; private set; }

        public IEnumerable<Observation> Observations { get; private set; }
        public Dictionary<string, Dictionary<string, ObservationDatePeriod>> ObservationDatePeriods { get; private set; }
        public Dictionary<string, int> ObservationIdIndex { get; private set; }

        public Dictionary<int, IEnumerable<int>> ObservationChangeIds { get; private set; }
        public IEnumerable<ObservationChange> ObservationChanges { get; private set; }

        public Dictionary<int, IEnumerable<int>> ObservationCommentIds { get; private set; }
        public IEnumerable<ObservationComment> ObservationComments { get; private set; }

        public IEnumerable<ActivitySite> ActivitySites { get; private set; }

        public IEnumerable<Note> Notes { get; private set; }

        public Dictionary<string, string> Sexes { get; private set; }

        public IEnumerable<int> AssignedActivityIds
        {
            get
            {
                return AssignedActivities.Select(a => a.ActivityId);
            }
        }
        public IEnumerable<AssignedActivity> AssignedActivities { get; private set; }

        public IEnumerable<string> AssignedSiteKeys
        {
            get
            {
                return AssignedSites.Select(x => x.Key);
            }
        }
        public IEnumerable<AssignedSite> AssignedSites { get; private set; }

        public List<Tuple<string, string>> FieldIds { get; private set; }

        public IEnumerable<Country> Countries { get; private set; }

        public List<Tuple<int, string>> UserNames { get; private set; }

        #region Private DataSource Members

        private Entity _context { get; set; }

        private DbDataReader _reader { get; set; }

        private List<TEntity> TranslateNextResultSet<TEntity>() where TEntity : class
        {
            List<TEntity> list = new List<TEntity>();
            var temp = ((IObjectContextAdapter)_context).ObjectContext.Translate<TEntity>(_reader);
            foreach (var item in temp)
                list.Add(item);
            _reader.NextResult();
            return list;
        }

        #endregion
    }
}
