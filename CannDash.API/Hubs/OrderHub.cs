using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CannDash.API.Hubs
{
    [HubName("orderHub")]
    public class OrderHub : Hub
    {
        //Note that Subscribe() method is to be called from JavaScript on client browser when user searches
        //a certain dispensary id so that user starts to get real time notifications about the dispensary. 
        //Similarly Unsubscribe() method is to stop getting notifications from the given dispensary.
        public Task Subscribe(string dispensaryId)
        {
            return Groups.Add(Context.ConnectionId, dispensaryId);
        }

        public Task Unsubscribe(string dispensaryId)
        {
            return Groups.Remove(Context.ConnectionId, dispensaryId);
        }
    }
}