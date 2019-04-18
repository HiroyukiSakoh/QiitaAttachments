using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzFuncDISample
{
    public class DocumentClientFunction
    {
        private readonly IDocumentClient documentClient;
        public DocumentClientFunction(IDocumentClient documentClient)
        {
            this.documentClient = documentClient;
        }
        [FunctionName(nameof(DocumentClientFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequest request,
            ILogger log)
        {
            var databaseAccount = await documentClient.GetDatabaseAccountAsync();
            return new OkObjectResult(new
            {
                databaseId = databaseAccount.Id,
            });
        }
    }
}
