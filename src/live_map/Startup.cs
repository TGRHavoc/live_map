using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static CitizenFX.Core.Native.API;


namespace live_map
{
    public class Startup
    {
        public static string RootPath = GetResourcePath(GetCurrentResourceName());

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            
            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddServerSentEvents();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.MapServerSentEvents("/updates");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
