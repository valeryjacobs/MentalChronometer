using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MentalChronometerHub
{
    public class MentalChronometerHub : Hub
    {
        Dictionary<string, List<int>> DeviceRegister = new Dictionary<string, List<int>>();

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void Eenofanderenaam(string deviceId, string payload)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.toDevice(payload);
        }



        public void Register(string deviceId)
        {
            if (!DeviceRegister.ContainsKey(deviceId))
                DeviceRegister.Add(deviceId, null);
            else
                DeviceRegister[deviceId] = null;
        }
    }   
}