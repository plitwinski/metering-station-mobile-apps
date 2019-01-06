using System;
using System.Collections.Generic;
using Autofac;

namespace MeteringStation.Mobile.Tests.Fixtures
{
    public class TestApp : App
    {
        public virtual IDictionary<Type, object> GetMocks()
            => new Dictionary<Type, object>();

        protected override ContainerBuilder GetContainerBuilder()
        {
            var containerBuilder = base.GetContainerBuilder();

            foreach (var mock in GetMocks())
                containerBuilder.Register(_ => mock.Value).As(mock.Key);

            return containerBuilder;
        }

        public void CallOnStart() => base.OnStart();
        public void CallOnResume() => base.OnResume();
    }
}
