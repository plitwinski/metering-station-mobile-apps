using MeteringStation.Mobile.Tests.Builders.Stubs;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace MeteringStation.Mobile.Tests.Builders
{
    internal class HttpMessageHandlerBuilder : BaseMockBuilder<FakeHttpMessageHandler>
    {
        public HttpMessageHandlerBuilder() : base(true) { }

        public HttpMessageHandlerBuilder SetupGet<TResponse>(string url, params TResponse[] responses)
        {
            var responseQueue = new Queue<TResponse>(responses);
            Mock.Setup(p => p.Send(It.Is<HttpRequestMessage>(req => req.RequestUri.OriginalString == url), 
                                   It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(responseQueue.Dequeue()))
                });
                
            return this;
        }
    }
}
