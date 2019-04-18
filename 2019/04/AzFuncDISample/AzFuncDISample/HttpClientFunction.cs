using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzFuncDISample
{
    public class HttpClientFunction
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly HttpClient httpClient1;
        private readonly HttpClient httpClient2;
        public HttpClientFunction(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            httpClient1 = this.httpClientFactory.CreateClient("httpstat");
            httpClient2 = this.httpClientFactory.CreateClient("httpstatWithPolly");
        }
        [FunctionName(nameof(HttpClientFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequest request,
            ILogger log)
        {
            string responseText1 = await httpClient1.GetStringAsync("/200");
            string responseText2 = null;
            try
            {
                responseText2 = await httpClient2.GetStringAsync("/429");
            }
            catch (Exception ex)
            {
                responseText2 = ex.ToString();
            }
            return new OkObjectResult(new
            {
                responseText1,
                responseText2,
            });
        }
    }
}
