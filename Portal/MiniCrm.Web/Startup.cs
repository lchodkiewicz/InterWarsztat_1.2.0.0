using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniCrm.Web.Startup))]
namespace MiniCrm.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
