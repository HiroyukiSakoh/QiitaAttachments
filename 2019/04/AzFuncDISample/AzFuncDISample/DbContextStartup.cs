using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

[assembly: FunctionsStartup(typeof(AzFuncDISample.DbContextStartup))]
namespace AzFuncDISample
{
    public class DbContextStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<SampleContext>((provider,options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString("SampleConnection"));
            });
        }
    }
    public class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions<SampleContext> options)
            : base(options)
        { }
        public DbSet<Sample> Samples { get; set; }
    }
    public class Sample
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
