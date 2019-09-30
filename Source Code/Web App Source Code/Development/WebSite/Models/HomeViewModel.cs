using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class HomeViewModel
    {
        public HomeObjects HomeObjects { get; private set; }

        public HomeViewModel()
        {
            HomeObjects = new HomeObjects(Helpers.Identity.CurrentUser.Id, IidCulture.CurrentLanguageId);
        }
    }
}