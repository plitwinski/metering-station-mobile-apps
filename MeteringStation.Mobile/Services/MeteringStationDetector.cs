using Autofac;
using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Extensions;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Services.Communication;
using MeteringStation.Mobile.Services.Dtos;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Services
{
    public class MeteringStationDetector : IMeteringStationDetector
    {
        private const string DiscoverySenderId = "bb407f65-e945-49b7-849b-925cee163de5";
        private const int Port = 8999;

        private readonly IEventAggregator eventAggregator;
        private readonly ILifetimeScope lifetimeScope;
        private readonly TimeSpan? discoveryTimeout;

        public MeteringStationDetector(IEventAggregator eventAggregator, 
            ILifetimeScope lifetimeScope,
            TimeSpan? discoveryTimeout = null)
        {
            this.eventAggregator = eventAggregator;
            this.lifetimeScope = lifetimeScope;
            this.discoveryTimeout = discoveryTimeout;
        }

        public async Task StartDiscoveryAsync()
        {
            await DiscoverDevicesAsync(discoveryTimeout ?? TimeSpan.FromSeconds(10));
        }

        private async Task DiscoverDevicesAsync(TimeSpan discoveryTimeout)
        {
            eventAggregator.Publish(new DetectionStartedEvent());
            using (var c = lifetimeScope.BeginLifetimeScope())
            {
                var broadcaster = c.Resolve<IBroadcaster>();
                broadcaster.Send(DiscoverySenderId, Port, OnDeviceDetected);
                await Task.Delay(discoveryTimeout);
                broadcaster.Close();
            }
            eventAggregator.Publish(new DetectionFinishedEvent());
        }

        private void OnDeviceDetected(IPEndPoint ip, string encryptedResponse)
        {
            var responseMessage = encryptedResponse.Decrypt();
            var response = JsonConvert.DeserializeObject<MeteringStationResponse>(responseMessage);
            eventAggregator
                .Publish(new DeviceDetectedEvent(response.ClientId, ip.Address));
        }
    }
}
