using System;
using System.Collections.Generic;
using System.Text;
using MeteringStation.Mobile.Services;
using Moq;

namespace MeteringStation.Mobile.Tests.Builders
{
    internal class MeteringStationDetectorBuilder
    {
        private readonly Mock<IMeteringStationDetector> _meteringStationDetectorMock;
        
        public MeteringStationDetectorBuilder()
        {
            _meteringStationDetectorMock = new Mock<IMeteringStationDetector>();
        }

        public Mock<IMeteringStationDetector> Mock => _meteringStationDetectorMock;

        public IMeteringStationDetector Build()
            => _meteringStationDetectorMock.Object;
    }
}
