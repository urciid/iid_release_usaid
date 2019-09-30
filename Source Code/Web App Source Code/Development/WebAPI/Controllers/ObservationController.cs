using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Models;

namespace IID.WebAPI.Controllers
{
    public class ObservationController : ApiController
    {

        [Authorize]
        [Route("api/Observation/Update")]
        public IEnumerable<p_api_update_observation_Result> Update([FromBody] p_api_update_observation_Result obs)
        {
            Entity context = new Entity();
            IEnumerable<p_api_update_observation_Result> results = context.p_api_update_observation(obs.observation_id, obs.indicator_id, obs.site_id, obs.begin_date, obs.end_date, obs.user_email).ToList();
            return results;
        }

        [Authorize]
        [Route("api/Observation/UpdateComment")]
        public IEnumerable<p_api_update_observation_comment_Result> UpdateComment([FromBody] p_api_update_observation_comment_Result c)
        {
            Entity context = new Entity();
            IEnumerable<p_api_update_observation_comment_Result> results = context.p_api_update_observation_comment(c.observation_comment_id, c.observation_id, c.comment, c.user_email).ToList();
            return results;
        }

        [Authorize]
        [Route("api/Observation/UpdateChange")]
        public IEnumerable<p_api_update_observation_change_Result> UpdateChange([FromBody] p_api_update_observation_change_Result c)
        {
            Entity context = new Entity();
            IEnumerable<p_api_update_observation_change_Result> results = context.p_api_update_observation_change(c.observation_change_id, c.observation_id, c.description, c.start_date, c.user_email).ToList();
            return results;
        }

        [Authorize]
        [Route("api/Observation/UpdateAttachment")]
        public async Task<IEnumerable<p_api_update_observation_attachment_Result>> UpdateAttachment()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                // Read form keys.
                int observationAttachmentId = Convert.ToInt32(provider.FormData["observation_attachment_id"]);
                int observationId = Convert.ToInt32(provider.FormData["observation_id"]);
                string userEmail = provider.FormData["user_email"];
                string attachmentFileName = provider.FormData["attachment_file_name"];

                // Read file.
                var file = provider.FileData.First();
                var bytes = File.ReadAllBytes(file.LocalFileName);

                using (Entity context = new Entity())
                {
                    // Upsert to database.
                    var result = context.p_api_update_observation_attachment(observationAttachmentId, observationId, attachmentFileName, bytes, userEmail).ToList();

                    // Delete file.
                    File.Delete(file.LocalFileName);

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [Authorize]
        [Route("api/Observation/UpdateEntry")]
        public IEnumerable<p_api_update_observation_entry_Result> UpdateEntry([FromBody] p_api_update_observation_entry_Result e)
        {
            Entity context = new Entity();
            IEnumerable<p_api_update_observation_entry_Result> results = context.p_api_update_observation_entry(e.observation_entry_id, e.observation_id, e.indicator_age_range_id, e.indicator_gender, e.numerator, e.denominator, e.count, e.rate, e.yes_no, e.user_email).ToList();
            return results;
        }

        [Authorize]
        [Route("api/Observation/GetAllData")]
        public ApiMyData GetAllData(string email, int languageid)
        {
            ApiMyData ApiData = new ApiMyData();
            
            using (Entity context = new Entity())
            {
                ApiData.Sites = context.p_api_get_mydata_sites(email, languageid).ToList();
          
                ApiData.Indicators = context.p_api_get_mydata_indicators_new(email, languageid).ToList();
            
                ApiData.Activities = context.p_api_get_mydata_activities(email, languageid).ToList();
            
                ApiData.SiteIndicators = context.p_api_get_mydata_site_indicators(email, languageid).ToList();
          
                ApiData.IndicatorAgePeriods = context.p_api_get_mydata_indicator_age_periods(email, languageid).ToList();
           
                ApiData.Observations = context.p_api_get_mydata_observations(email, languageid).ToList();
           
                ApiData.ObservationEntries = context.p_api_get_mydata_observation_entries(email, languageid).ToList();
           
                ApiData.ObservationChanges = context.p_api_get_mydata_observation_changes(email, languageid).ToList();
           
                ApiData.ObservationComments = context.p_api_get_mydata_observation_comments(email, languageid).ToList();
           
                ApiData.ObservationAttachments = context.p_api_get_mydata_observation_attachments(email, languageid).ToList();
            }

            return ApiData;
        }


    }

}