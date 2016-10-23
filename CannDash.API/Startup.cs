using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(CannDash.API.Startup))]

namespace CannDash.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            //app.MapSignalR();

            //Add SignalR
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);

                // configure signalR
                HubConfiguration hubConfiguration = new HubConfiguration()
                {
                    EnableJavaScriptProxies = false,
                    EnableDetailedErrors = true
                };

                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
