using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Mvc;
using static CitizenFX.Core.Native.API;

namespace live_map
{
    public class HomeController : Controller
    {
        private IServerSentEventsService eventsService;

        public HomeController(IServerSentEventsService events)
        {
            this.eventsService = events;
        }

        [HttpGet]
        public string Index()
        {
            return "Hello world!";
        }

    }
}