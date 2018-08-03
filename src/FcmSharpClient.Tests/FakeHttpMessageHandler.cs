using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FcmSharpClient.Tests
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public int CallCount;
        public List<HttpRequestMessage> Requests = new List<HttpRequestMessage>();
        public List<string> RequestBodies = new List<string>();
        public List<HttpResponseMessage> Responses = new List<HttpResponseMessage>();
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            RequestBodies.Add(await request.Content.ReadAsStringAsync());
            return Responses[CallCount++];
        }
    }
}
