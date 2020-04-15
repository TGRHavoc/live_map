using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace live_map
{
    public class Program : BaseScript
    {
        public Program()
        {
            Tick += FirstTick;
        }

        private async Task FirstTick()
        {
            Tick -= FirstTick;

            // screw TLS certs especially as they require manually deploying a root list.. really?
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            var host = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IServer, HttpServer>();
                    services.AddSingleton<IHostedService, TestService>();
                    services.AddSingleton<ConsoleLog>();
                })
                .UseStartup<Startup>()
                .Build();

            host.Start();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await Task.Delay(0);
        }


    }
}
