using MeteringStation.Mobile.Events;
using MeteringStation.Mobile.Messaging;
using MeteringStation.Mobile.Tests.Builders;
using MeteringStation.Mobile.ViewModels;
using MeteringStation.Mobile.ViewModels.Dtos;
using MeteringStation.Mobile.ViewModels.Models;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MeteringStation.Mobile.Tests.ViewModels
{
    public class MetersViewModelTests
    {
        [Test]
        public void WhenDetectionStartedEventSend_IsLoadingSet()
        {
            var ea = new EventAggregator();
            var httpClient = new HttpClient(new HttpMessageHandlerBuilder().Build());
            var target = new MetersViewModel(ea, httpClient);

            ea.Publish(new DetectionStartedEvent());

            Assert.IsTrue(target.IsLoading);
        }

        [Test]
        public void WhenDetectionFinishedEventSend_IsLoadingUnset()
        {
            var ea = new EventAggregator();
            var httpClient = new HttpClient(new HttpMessageHandlerBuilder().Build());
            var target = new MetersViewModel(ea, httpClient);

            ea.Publish(new DetectionStartedEvent());
            ea.Publish(new DetectionFinishedEvent());

            Assert.IsFalse(target.IsLoading);
        }

        [Test]
        public void WhenNoReadingsExist_DeviceDetectedEventSend_ReadingsUpdated()
        {
            var ea = new EventAggregator();
            var response = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test", PM25="1",  PM10="2" }
                };
            var handler = new HttpMessageHandlerBuilder()
                .SetupGet<ReadingDto[]>("http://1.2.3.4:8080/v1/readings", response)
                .Build();

            var target = new MetersViewModel(ea, new HttpClient(handler));

            ea.Publish(new DeviceDetectedEvent("id", IPAddress.Parse("1.2.3.4")));

            var result = target.Readings.Single();

            Assert.AreEqual("id", result.ClientId);
            Assert.AreEqual("unit-test", result.DeviceName);
            Assert.AreEqual("1", result.PM25);
            Assert.AreEqual("2", result.PM10);
        }

        [Test]
        public void WhenNoReadingsExist_DeviceDetectedEventSend_WithMultipleResponses_ReadingsUpdated()
        {
            var ea = new EventAggregator();
            var response = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test", PM25="1",  PM10="2" },
                    new ReadingDto { DeviceName = "unit-test2", PM25="10",  PM10="20" }
                };
            var handler = new HttpMessageHandlerBuilder()
                .SetupGet<ReadingDto[]>("http://1.2.3.4:8080/v1/readings", response)
                .Build();

            var target = new MetersViewModel(ea, new HttpClient(handler));

            ea.Publish(new DeviceDetectedEvent("id", IPAddress.Parse("1.2.3.4")));

            var firstResult = target.Readings.First();
            var lastResult = target.Readings.Last();

            Assert.AreEqual(2, target.Readings.Count);

            //TODO add object comparison
            Assert.AreEqual("id", firstResult.ClientId);
            Assert.AreEqual("unit-test", firstResult.DeviceName);
            Assert.AreEqual("1", firstResult.PM25);
            Assert.AreEqual("2", firstResult.PM10);

            Assert.AreEqual("id", lastResult.ClientId);
            Assert.AreEqual("unit-test2", lastResult.DeviceName);
            Assert.AreEqual("10", lastResult.PM25);
            Assert.AreEqual("20", lastResult.PM10);
        }

        [Test]
        public void WhenNoReadingsExist_MultipleDeviceDetectedEventSend_ReadingsUpdated()
        {
            var ea = new EventAggregator();
            var firstDevice = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test-1", PM25="1",  PM10="2" }
                };
            var secondDevice = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test-2", PM25="10",  PM10="20" }
                };
            var handler = new HttpMessageHandlerBuilder()
                .SetupGet<ReadingDto[]>("http://1.2.3.4:8080/v1/readings", firstDevice)
                .SetupGet<ReadingDto[]>("http://1.2.3.5:8080/v1/readings", secondDevice)
                .Build();

            var target = new MetersViewModel(ea, new HttpClient(handler));
            ea.Publish(new DeviceDetectedEvent("id1", IPAddress.Parse("1.2.3.4")));
            ea.Publish(new DeviceDetectedEvent("id2", IPAddress.Parse("1.2.3.5")));

            var firstResult = target.Readings.First();
            var lastResult = target.Readings.Last();

            Assert.AreEqual(2, target.Readings.Count);

            //TODO add object comparison
            Assert.AreEqual("id1", firstResult.ClientId);
            Assert.AreEqual("unit-test-1", firstResult.DeviceName);
            Assert.AreEqual("1", firstResult.PM25);
            Assert.AreEqual("2", firstResult.PM10);

            Assert.AreEqual("id2", lastResult.ClientId);
            Assert.AreEqual("unit-test-2", lastResult.DeviceName);
            Assert.AreEqual("10", lastResult.PM25);
            Assert.AreEqual("20", lastResult.PM10);
        }

        [Test]
        public void WhenReadingsExist_DeviceDetectedEventSend_ReadingsUpdated()
        {
            var ea = new EventAggregator();
            var response = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test", PM25="1",  PM10="2" }
                };
            var handler = new HttpMessageHandlerBuilder()
                .SetupGet<ReadingDto[]>("http://1.2.3.4:8080/v1/readings", response)
                .Build();

            var target = new MetersViewModel(ea, new HttpClient(handler));
            target.Readings.Add(new Reading() { ClientId = "existingId", DeviceName = "existing", PM25 = "10", PM10 = "20" });

            ea.Publish(new DeviceDetectedEvent("id", IPAddress.Parse("1.2.3.4")));

            var result = target.Readings.Last();

            Assert.AreEqual("id", result.ClientId);
            Assert.AreEqual("unit-test", result.DeviceName);
            Assert.AreEqual("1", result.PM25);
            Assert.AreEqual("2", result.PM10);
        }

        [Test]
        public void WhenReadingsExist_Refresh_ReadingsUpdated()
        {
            var ea = new EventAggregator();
            var initialResponse = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test", PM25="1",  PM10="2" }
                };
            var updateResponse = new ReadingDto[]
                {
                    new ReadingDto { DeviceName = "unit-test", PM25="10",  PM10="20" }
                };
            var handler = new HttpMessageHandlerBuilder()
                .SetupGet("http://1.2.3.4:8080/v1/readings", initialResponse, updateResponse)
                .Build();

            var target = new MetersViewModel(ea, new HttpClient(handler));
            ea.Publish(new DeviceDetectedEvent("id", IPAddress.Parse("1.2.3.4")));

            target.Refresh.Execute(null);

            var result = target.Readings.Single();

            Assert.AreEqual("id", result.ClientId);
            Assert.AreEqual("unit-test", result.DeviceName);
            Assert.AreEqual("10", result.PM25);
            Assert.AreEqual("20", result.PM10);
        }
    }
}
