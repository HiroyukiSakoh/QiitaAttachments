using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AzFuncDISample
{
    public class DbContextFunction
    {
        private readonly SampleContext dbContext;
        public DbContextFunction(SampleContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [FunctionName(nameof(DbContextFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequest request,
            ILogger log)
        {
            dbContext.Database.OpenConnection();
            using (var connection = dbContext.Database.GetDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM sys.tables";
                var count = (int)await command.ExecuteScalarAsync();
                return new OkObjectResult(count);
            }
        }
    }
}
