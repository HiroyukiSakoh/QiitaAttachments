using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

[assembly: FunctionsStartup(typeof(AzFuncDISample.DocumentClientStartup))]
namespace AzFuncDISample
{
    public class DocumentClientStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IDocumentClient>(provider => {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var accountEndpoint = new Uri(configuration.GetValue<string>("Cosmos:AccountEndpoint"));
                var accountKey = configuration.GetValue<string>("Cosmos:AccountKey");
                var connectionPolicy = new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                };
                connectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 5;
                connectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 60;
                return new DocumentClient(accountEndpoint, accountKey, connectionPolicy);
            });

        }
    }
}
