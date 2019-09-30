using System.Web.Mvc;

namespace IID.WebSite.Controllers
{
    public class OfflineController : BaseController
    {
        [NoCache()]
        public ActionResult Index()
        {
            return View();
        }

        [NoCache()]
        public ActionResult OfflineSync()
        {
            return View(new Models.OfflineSync());
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Activity()
        {
            return View();
        }

        public ActionResult Indicator()
        {
            return View();
        }

        public ActionResult Site()
        {
            return View();
        }

        public ActionResult ObservationView()
        {
            return View();
        }

        public ActionResult ObservationRecord()
        {
            return View();
        }

        public ActionResult Redirect()
        {
            return View();
        }

        public ActionResult CoachReport()
        {
            return View();
        }

        public ActionResult OnlineSync()
        {
            return View();
        }
    }
}