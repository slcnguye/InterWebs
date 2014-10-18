using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(InterWebs.Startup))]
namespace InterWebs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR(new HubConfiguration { EnableJSONP = true }).UseCors(CorsOptions.AllowAll);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
