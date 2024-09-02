using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Http;

namespace bbt.gateway.common.Http
{
    public class ProxyByPassHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                UseProxy = false,
            };
            return httpClientHandler;
        }
    }
}
