using MeteringStation.Mobile.Services.Communication;
using Moq;
using System;
using System.Net;

namespace MeteringStation.Mobile.Tests.Builders
{
    internal class BroadcasterBuilder : BaseMockBuilder<IBroadcaster>
    {
        public BroadcasterBuilder SetupSend(IPEndPoint ip, string encryptedResponse)
        {
            Mock.Setup(p => p.Send(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action<IPEndPoint, string>>()))
                .Callback<string, int, Action<IPEndPoint, string>>((_, __, callback) => callback(ip, encryptedResponse));
            return this;
        }
    }
}
