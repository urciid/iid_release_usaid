using System.Web.Mvc;

namespace IID.WebSite.Controllers
{
    public class ManifestController : BaseController
    {
        public ActionResult Index()
        {
            Response.ContentType = "text/cache-manifest";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            return View();
        }
    }
}