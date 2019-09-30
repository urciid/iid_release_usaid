using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IID.WebSite.Startup))]
namespace IID.WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
