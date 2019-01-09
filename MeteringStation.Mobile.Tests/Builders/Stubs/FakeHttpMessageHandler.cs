using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MeteringStation.Mobile.Tests.Builders.Stubs
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Send(request, cancellationToken);

        public virtual Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("For testing purposes only");
        }
    }
}
