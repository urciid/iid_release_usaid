using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.BusinessLayer.Models;

namespace IID.WebAPI.Controllers
{
    public class SiteController : ApiController
    {


        public IEnumerable<p_api_change_get_Result> Get(string userid, int languageid)
        {

            var context = new Entity();

            IEnumerable<p_api_change_get_Result> results = context.p_api_change_get(userid, languageid);

            return results;

        }


        //[Route("api/Site/MyData")]
        //public IEnumerable<p_api_get_mydata_Result> GetMyData(string email, int languageid)
        //{
        //    var context = new Entity();

        //    IEnumerable<p_api_get_mydata_Result> results = context.p_api_get_mydata(email, languageid);

        //    return results;

        //}


        [Route("api/Site/Countries")]
        public IHttpActionResult GetCountries(int? countryId = null, bool? active = null, byte languageId = 1)
        {
            using (Entity db = new Entity())
                return Ok(db.t_country.Where(e => (countryId == null || e.country_id == countryId) &&
                                                  (active == null || e.active == active))
                                      .ToList()
                                      .Select(e => new
                                      {
                                          CountryId = e.country_id,
                                          Name = e.name,
                                          AdministrativeDivision1 = e.get_first_level_administrative_division_translated(languageId),
                                          AdministrativeDivision2 = e.get_second_level_administrative_division_translated(languageId),
                                          AdministrativeDivision3 = e.get_third_level_administrative_division_translated(languageId),
                                          AdministrativeDivision4 = e.get_fourth_level_administrative_division_translated(languageId),
                                          Active = e.active,
                                          ParentSiteType = e.parent_site_type_fieldid
                                      })
                                      .OrderBy(e => e.Name));
        }

        [Route("api/Site/AdministrativeDivisions")]
        public IHttpActionResult GetAdministrativeDivisions(int countryId, int? parentDivisionId = null, byte languageId = 1)
        {
            using (Entity db = new Entity())
                return Ok(db.t_administrative_division
                            .Where(e => e.country_id == countryId &&
                                        e.parent_administrative_division_id == parentDivisionId)
                            .ToList()
                            .Select(e => new
                            {
                                AdministrativeDivisionId = e.administrative_division_id,
                                ParentAdministrativeDivisionId = e.parent_administrative_division_id,
                                Name = e.get_name_translated(languageId)
                            })
                            .OrderBy(e => e.Name));
        }

        [Route("api/Site/SiteTypes")]
        public IHttpActionResult GetSiteTypes(int countryId, byte languageId)
        {
            using (Entity db = new Entity())
            {
                string parentFieldId = db.t_country.Find(countryId).parent_site_type_fieldid;

                if (parentFieldId == null)
                    return null;

                return Ok(db.t_fieldid
                            .Where(e => e.parent_fieldid == parentFieldId)
                            .ToList()
                            .Select(e => new
                            {
                                FieldId = e.fieldid,
                                Value = e.get_value_translated(languageId)
                            })
                            .OrderBy(e => e.Value));
            }
        }

        [Route("api/Site/Search")]
        public IHttpActionResult PostSearch([FromBody] SiteSearchCriteria criteria)
        {
            if (criteria == null)
                criteria = new SiteSearchCriteria();

            using (Entity db = new Entity())
            {
                return Ok(db.v_site
                            .Where(e => (criteria.CountryId == null || criteria.CountryId == e.country_id) &&
                                        ((criteria.AdministrativeDivisionId1 == null || criteria.AdministrativeDivisionId1.Value == e.administrative_division_id_1) &&
                                            (criteria.AdministrativeDivisionId2 == null || criteria.AdministrativeDivisionId2.Value == e.administrative_division_id_2) &&
                                            (criteria.AdministrativeDivisionId3 == null || criteria.AdministrativeDivisionId3.Value == e.administrative_division_id_3) &&
                                            (criteria.AdministrativeDivisionId4 == null || criteria.AdministrativeDivisionId4.Value == e.administrative_division_id_4)) &&
                                            (criteria.SiteTypeFieldId == null || criteria.SiteTypeFieldId == e.site_type_fieldid))
                            .ToList()
                            .Select(e => new SiteSearchResult
                            {
                                SiteId = e.site_id,
                                SiteName = e.get_name_translated(criteria.LanguageId),
                                CountryId = e.country_id,
                                CountryName = e.get_country_name_translated(criteria.LanguageId),
                                AdministrativeDivisionId1 = e.administrative_division_id_1,
                                AdministrativeDivisionName1 = e.administrative_division_name_1,
                                AdministrativeDivisionId2 = e.administrative_division_id_2,
                                AdministrativeDivisionName2 = e.administrative_division_name_2,
                                AdministrativeDivisionId3 = e.administrative_division_id_3,
                                AdministrativeDivisionName3 = e.administrative_division_name_3,
                                AdministrativeDivisionId4 = e.administrative_division_id_4,
                                AdministrativeDivisionName4 = e.administrative_division_name_4,
                                SiteTypeValue = e.site_type_value,
                                QIIndexScore = e.qi_index_score_value
                            })
                            .OrderBy(e => e.SiteName));
            }
        }
    }
}