using System.Web.Routing;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(InterWebs.Startup))]
namespace InterWebs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }
    }
}
