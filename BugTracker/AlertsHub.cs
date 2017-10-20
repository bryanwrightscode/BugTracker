using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BugTracker
{
    public class AlertsHub : Hub
    {
        public void Send(string alert)
        {
            Clients.All.Broadcast(alert);
        }
        public void Send(string alert, string url)
        {
            Clients.All.Broadcast(alert, url);
        }
    }
}