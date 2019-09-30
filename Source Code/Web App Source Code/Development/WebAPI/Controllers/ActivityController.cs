using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Models;

namespace IID.WebAPI.Controllers
{
    public class ActivityController : ApiController
    {
        [Route("api/Activity/Search")]
        public ICollection<ActivitySearchResult> PostSearch([FromBody] ActivitySearchCriteria criteria)
        {
            using (Entity db = new Entity())
            {
                // Activities <--> Technical Area Subtypes is a many to many relationship.
                IQueryable<t_activity> activities;
                if (criteria.TechnicalAreaSubtypeFieldIds != null && criteria.TechnicalAreaSubtypeFieldIds.Length > 0)
                {
                    var fieldids = db.t_fieldid.Where(e => (e.parent_fieldid == FieldIdParentTypes.TechnicalAreaSubtype));
                    fieldids = fieldids.Where(e => criteria.TechnicalAreaSubtypeFieldIds.Contains(e.fieldid));
                    activities = fieldids.SelectMany(e => e.technical_area_subtype_activities);
                }
                else
                {
                    activities = db.t_activity;
                }
                if (criteria.CountryIds != null && criteria.CountryIds.Length > 0)
                    activities = activities.Where(e => criteria.CountryIds.Contains(e.country_id));
                if (criteria.TechnicalAreaFieldIds != null && criteria.TechnicalAreaFieldIds.Length > 0)
                    activities = activities.Where(e => criteria.TechnicalAreaFieldIds.Contains(e.technical_area_fieldid));
                switch (criteria.Status)
                {
                    case "Active":
                        activities = activities.Where(e => !e.end_date.HasValue);
                        break;

                    case "Completed":
                        activities = activities.Where(e => e.end_date.HasValue && e.end_date <= DateTime.Now);
                        break;
                }

                return activities
                        .ToList()
                        .Select(e => new ActivitySearchResult
                        {
                            // Second select statement to convert array afer SQL is executed.
                            ActivityId = e.activity_id,
                            ActivityName = e.get_name_translated(criteria.LanguageId),
                            CountryId = e.country_id,
                            CountryName = e.country.get_name_translated(criteria.LanguageId),
                            ProjectId = e.project_id,
                            ProjectName = e.project.get_name_translated(criteria.LanguageId),
                            TechnicalAreaValue = e.technical_area?.value,
                            //TechnicalAreaSubtypeValues = e.TechnicalAreaSubTypeValues.ToArray(),
                            StartDate = e.start_date?.ToShortDateString(),
                            EndDate = e.end_date?.ToShortDateString()
                        })
                        .OrderBy(e => e.ActivityName)
                        .ToList();
            }
        }

        [Route("api/Activity/SearchTechnicalAreas")]
        public IHttpActionResult PostSearchTechnicalAreas([FromBody] ActivitySearchCriteria criteria)
        {
            using (Entity db = new Entity())
            {
                IQueryable<t_activity> activities = db.t_activity.AsQueryable();
                if (criteria.CountryIds != null && criteria.CountryIds.Length > 0)
                    activities = activities.Where(e => criteria.CountryIds.Contains(e.country_id));
                return Ok(
                    activities
                        .Where(e => e.technical_area_fieldid != null)
                        .Select(e => new
                            {
                                fieldid = e.technical_area_fieldid,
                                value = e.technical_area.value
                            })
                        .Distinct()
                        .OrderBy(e => e.value)
                        .ToList()
                    );
            }
        }

        [Route("api/Activity/SearchTechnicalAreaSubtypes")]
        public IHttpActionResult PostSearchTechnicalAreaSubtypes([FromBody] ActivitySearchCriteria criteria)
        {
            using (Entity db = new Entity())
            {
                IQueryable<t_activity> activities = db.t_activity.AsQueryable();
                if (criteria.CountryIds != null && criteria.CountryIds.Length > 0)
                    activities = activities.Where(e => criteria.CountryIds.Contains(e.country_id));
                if (criteria.TechnicalAreaFieldIds != null && criteria.TechnicalAreaFieldIds.Length > 0)
                    activities = activities.Where(e => criteria.TechnicalAreaFieldIds.Contains(e.technical_area_fieldid));
                return Ok(
                    activities
                        .SelectMany(e => e.technical_area_subtypes.Select(f => new
                            {
                                f.fieldid,
                                f.value
                            })
                        )
                        .Distinct()
                        .OrderBy(e => e.value)
                        .ToList()
                    );
            }
        }
    }
}