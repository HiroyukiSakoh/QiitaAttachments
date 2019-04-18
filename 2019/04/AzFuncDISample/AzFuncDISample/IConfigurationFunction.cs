using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzFuncDISample
{
    public class IConfigurationFunction
    {
        private readonly IConfiguration configuration;
        public IConfigurationFunction(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [FunctionName(nameof(IConfigurationFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequest request,
            ILogger log)
        {
            var configValue = configuration.GetValue<string>("key");
            return new OkObjectResult(new
            {
                configValue,
            });
        }
    }
}
