using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FungeyeApp.Startup))]
namespace FungeyeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
