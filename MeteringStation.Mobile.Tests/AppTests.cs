using MeteringStation.Mobile.Pages;
using MeteringStation.Mobile.Tests.Builders;
using MeteringStation.Mobile.Tests.Fixtures;
using Moq;
using NUnit.Framework;
using System;
using Xamarin.Forms.Mocks;

namespace MeteringStation.Mobile.Tests
{
    public class AppTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            MockForms.Init();
        }

        [Test]
        public void WhenAppInitiated_MetersAppLoaded()
        {
            var app = new AppFixture()
                .OverrideWith(new MeteringStationDetectorBuilder().Build())
                .Create();

            Assert.IsInstanceOf<MetersPage>(app.MainPage);
        }

        [Test]
        public void WhenAppOnStart_StartDetectingMeters()
        {
            var msdBuilder = WhenAppDiscoveryAction(app => app.CallOnStart());
            msdBuilder.Mock.Verify(p => p.StartDiscoveryAsync(), Times.Once);
        }

        [Test]
        public void WhenAppOnResume_StartDetectingMeters()
        {
            var msdBuilder = WhenAppDiscoveryAction(app => app.CallOnResume());
            msdBuilder.Mock.Verify(p => p.StartDiscoveryAsync(), Times.Once);
        }

        private MeteringStationDetectorBuilder WhenAppDiscoveryAction(Action<TestApp> testAction)
        {
            var msdBuilder = new MeteringStationDetectorBuilder();

            var app = new AppFixture()
                .OverrideWith(msdBuilder.Build())
                .Create();

            testAction(app);

            return msdBuilder;
        }
    }
}
