using System;
using System.Collections.Generic;
using Moq;

namespace MeteringStation.Mobile.Tests.Fixtures
{
    internal class AppFixture
    {
        private readonly IDictionary<Type, object> _mocks;

        public AppFixture()
        {
            _mocks = new Dictionary<Type, object>();
        }

        public AppFixture OverrideWith<TRegistration>(TRegistration instance)
        {
            _mocks.Add(typeof(TRegistration), instance);
            return this;
        }

        public TestApp Create()
        {
            var instanceMock = new Mock<TestApp>() { CallBase = true };
            instanceMock.Setup(p => p.GetMocks()).Returns(_mocks);
            return instanceMock.Object;
        }
    }
}
