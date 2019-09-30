using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.WebSite.Models;
using IID.WebSite.Helpers;
using Newtonsoft.Json;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class ChartController : BaseController
    {
        [HttpGet()]
        public ActionResult Activity(int id)
        {
            var parameters = new ChartParameters(id);
            return View("Index", new ChartModel(parameters));
        }

        [HttpGet()]
        public ActionResult Indicator(int id)
        {
            var parameters = new ChartParameters(id, null, false, 0);
            return View("Index", new ChartModel(parameters));
        }

        [HttpGet()]
        public ActionResult Observation(int indicatorId, int siteId)
        {
            var parameters = new ChartParameters(indicatorId, null, false, 0);
            var model = new ChartModel(parameters);
            model.RefererSiteId = siteId;
            return View("Index", model);
        }


        [HttpGet()]
        public ActionResult MultiCountry(int id)
        {
            var parameters = new ChartParameters(id, null, false, 0);
            var model = new ChartModel(parameters, "rpc");
           // model.RefererSiteId = siteId;
            return View(model);
        }

        [HttpGet()]
        public ActionResult Load(int userId, string chartName)
        {
            using (Entity context = new Entity())
            {
                byte[] bytes = Convert.FromBase64String(chartName);
                string decoded = System.Text.Encoding.UTF8.GetString(bytes);

                var userChart = context.t_user_favorite_chart.Find(new object[] { userId, decoded });
                if (userChart == null)
                    throw new ArgumentException(String.Format("Chart not found! userId: {0}, chartName: {1}", userId, chartName));

                ChartParameters parameters = ChartParameters.DeserializeXml(userChart);
                var model = new ChartModel(parameters, parameters.ReportClassFieldId);
                model.ChartName = userChart.chart_name;

                if (parameters.ReportClassFieldId == null || parameters.ReportClassFieldId.Trim().Length == 0)             
                {
                    return View("Index", model);
                }
                else
                {
                    return View("MultiCountry", model);
                }
                
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Modify(int indicatorId, int? siteId, string criterias, bool addMedian, int activeCriteria, string chartName, string reportClassFieldId = "")
        {
            var criteriaArray = JsonConvert.DeserializeObject<ChartCriteria[]>(criterias);
            var parameters = new ChartParameters(indicatorId, criteriaArray, addMedian, activeCriteria);
            var model = new ChartModel(parameters);
            model.ChartName = chartName;
            model.RefererSiteId = siteId;
            return View("Index", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyMultiCountry(int indicatorId, int? siteId, string criterias, bool addMedian, int activeCriteria, string chartName, string reportClassFieldId = "")
        {
            var criteriaArray = JsonConvert.DeserializeObject<ChartCriteria[]>(criterias);
            var parameters = new ChartParameters(indicatorId, criteriaArray, addMedian, activeCriteria);
            var model = new ChartModel(parameters, reportClassFieldId);
            model.ChartName = chartName;
            model.RefererSiteId = siteId;
            return View("MultiCountry", model);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string chartName, int indicatorId, int? siteId, string criterias, bool addMedian, int activeCriteria, string reportClassFieldId = "")
        {
            var criteriaArray = JsonConvert.DeserializeObject<ChartCriteria[]>(criterias);
            var parameters = new ChartParameters(indicatorId, criteriaArray, addMedian, activeCriteria, reportClassFieldId);

            using (Entity context = new Entity())
            {
                var userChart = context.t_user_favorite_chart.Find(new object[] { CurrentUser.Id, chartName });
                if (userChart == null)
                {
                    userChart = new t_user_favorite_chart();
                    userChart.user_id = CurrentUser.Id;
                    userChart.chart_name = chartName;
                    context.t_user_favorite_chart.Add(userChart);
                }
                userChart.chart_parameters = ChartParameters.SerializeToXml(parameters);
                userChart.updated_date = DateTime.Now;
                context.SaveChanges();

                var chartModel = new ChartModel(parameters);
                chartModel.ChartName = chartName;
                chartModel.RefererSiteId = siteId;

                return View("Index", chartModel);
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string chartName)
        {
            using (Entity context = new Entity())
            {
                var userChart = context.t_user_favorite_chart.Find(new object[] { CurrentUser.Id, chartName });
                if (userChart == null)
                    return GetJsonResult(false, String.Format("The chart '{0}' does not exist for user {1} ({2}).", chartName, CurrentUser.Id, CurrentUser.UserName));

                context.t_user_favorite_chart.Remove(userChart);
                context.SaveChanges();
                return GetJsonResult(true, null);
            }
        }
    }
}