using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Lib.AspNetCore.ServerSentEvents;

namespace live_map
{
    public class TestService : BackgroundService
    {

        private IServerSentEventsService eventsService;

        public TestService(IServerSentEventsService events)
        {
            this.eventsService = events;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested){
                Console.WriteLine("Sending test!");

                await eventsService.SendEventAsync(string.Format("test {0}", DateTime.UtcNow));
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

    }
}