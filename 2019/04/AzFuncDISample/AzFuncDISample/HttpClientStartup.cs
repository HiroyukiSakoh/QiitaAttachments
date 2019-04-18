using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(AzFuncDISample.HttpClientStartup))]
namespace AzFuncDISample
{
    public class HttpClientStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = builder.Services
                .Where(s => s.ServiceType == typeof(IConfiguration)).First()
                .ImplementationInstance as IConfiguration;

            //HttpClient
            builder.Services.AddHttpClient("httpstat", httpClient =>
            {
                var hostName = configuration.GetValue<string>("Test:HostName");
                httpClient.BaseAddress = new Uri($"https://{hostName}/");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() => GetHttpMessageHandler());

            //HttpClient with Polly
            builder.Services.AddHttpClient("httpstatWithPolly", httpClient =>
            {
                var hostName = configuration.GetValue<string>("Test:HostName");
                httpClient.BaseAddress = new Uri($"https://{hostName}/");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .SetHandlerLifetime(System.Threading.Timeout.InfiniteTimeSpan)
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || (int)msg.StatusCode == 429)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)))
            .ConfigurePrimaryHttpMessageHandler(() => GetHttpMessageHandler());
        }

        private static HttpMessageHandler GetHttpMessageHandler()
        {
#if DEBUG
            return new HttpClientHandler()
            {
                Proxy = new System.Net.WebProxy("http://localhost:8888"),
                UseProxy = true,
            };
#else
            return new HttpClientHandler()
            {
            };
#endif
        }
    }
}
