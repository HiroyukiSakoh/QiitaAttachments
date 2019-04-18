using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

[assembly: WebJobsStartup(typeof(AzFuncDISample.DbContextStartup))]
namespace AzFuncDISample
{
    public class DbContextStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = builder.Services
                .Where(s => s.ServiceType == typeof(IConfiguration)).First()
                .ImplementationInstance as IConfiguration;

            builder.Services.AddDbContext<SampleContext>(options =>
            {
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
