using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Owin.Builder;
using Owin;

namespace mPower.WebApi.Extensions
{
    public static class SignalR2Extension
    {
        public static IApplicationBuilder UseAppBuilder(
           this IApplicationBuilder app,
           Action<IAppBuilder> configure)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var appBuilder = new AppBuilder();
                    appBuilder.Properties["builder.DefaultApp"] = next;

                    configure(appBuilder);
                    return appBuilder.Build<Func<IDictionary<string, object>, Task>>();
                });
            });

            return app;
        }

        public static void UseSignalR2(this IApplicationBuilder app)
        {
            app.UseAppBuilder(appBuilder => appBuilder.MapSignalR("/signalr", new HubConfiguration()));
        }
    }
}
