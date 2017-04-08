using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MentalChronometerHub
{
    public class MentalChronometerHub : Hub
    {
        Dictionary<string, List<int>> DeviceRegister = new Dictionary<string, List<int>>();
        public override Task OnConnected()
        {
            var deviceId = Context.QueryString["DeviceId"];

            // Register(deviceId);

            return base.OnConnected();
        }

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

        public void Init()
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.init();
        }

        public void Start()
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.start();
        }

        public void Stop()
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.stop();
        }

        public void Reset()
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.reset();
        }

        public void SensorMessage(string deviceId, string timestamp, double temp, double lumen)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.sensorMessage(deviceId, timestamp, temp, lumen);
        }

        public void ChronometricMeasurement(string deviceId, int latency, int duration)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.chronometricMeasurement(deviceId, latency, duration);
        }

        public void Register(string deviceId, string deviceName)
        {
            if (deviceId == null) deviceId = "0";

            if (!DeviceRegister.ContainsKey(deviceId))
                DeviceRegister.Add(deviceId, null);
            else
                DeviceRegister[deviceId] = null;

            Clients.All.register(deviceId, deviceName);
        }
    }
}