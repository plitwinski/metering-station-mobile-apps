using Autofac;
using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Services;
using MeteringStation.Mobile.Services.Dtos;
using MeteringStation.Mobile.Tests.Builders;
using MeteringStation.Mobile.Extensions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Tests.Services
{
    public class MeteringStationDetectorTests
    {
        [Test]
        public async Task WhenStartDiscoveryAsync_OneResponse_DeviceDetectedEventSent()
        {
            var broadcaster = new BroadcasterBuilder()
                .SetupSend(new IPEndPoint(IPAddress.Parse("1.2.3.4"), 1234), 
                            JsonConvert.SerializeObject(new MeteringStationResponse {
                                ClientId = "1234"
                            }).Encrypt())
                .Build();

            var eaBuilder = new EventAggregatorBuilder();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => broadcaster);

            var target = new MeteringStationDetector(eaBuilder.Build(), containerBuilder.Build(), TimeSpan.FromMilliseconds(200));
            await target.StartDiscoveryAsync();
            eaBuilder.Mock.Verify(p => p.Publish(It.Is<DeviceDetectedEvent>(x => x.Id == "1234" && x.Ip.ToString() == "1.2.3.4")), Times.Once);
        }

        [Test]
        public async Task WhenStartDiscoveryAsync_DetectionStartedEventSent()
        {
            var eaBuilder = await WhenStartDiscoveryAsync_NoResponse();
            eaBuilder.Mock.Verify(p => p.Publish(It.IsAny<DetectionStartedEvent>()), Times.Once);
        }

        [Test]
        public async Task WhenStartDiscoveryAsync_DetectionFinishedEventSent()
        {
            var eaBuilder = await WhenStartDiscoveryAsync_NoResponse();
            eaBuilder.Mock.Verify(p => p.Publish(It.IsAny<DetectionFinishedEvent>()), Times.Once);
        }

        [Test]
        public async Task WhenStartDiscoveryAsync_NoResponse_NoDeviceDetectedEventSent()
        {
            var eaBuilder = await WhenStartDiscoveryAsync_NoResponse();
            eaBuilder.Mock.Verify(p => p.Publish(It.IsAny<DeviceDetectedEvent>()), Times.Never);
        }

        private async Task<EventAggregatorBuilder> WhenStartDiscoveryAsync_NoResponse()
        {
            var eaBuilder = new EventAggregatorBuilder();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => new BroadcasterBuilder().Build());

            var target = new MeteringStationDetector(eaBuilder.Build(), containerBuilder.Build(), TimeSpan.FromMilliseconds(200));
            await target.StartDiscoveryAsync();

            return eaBuilder;
        }
    }
}
