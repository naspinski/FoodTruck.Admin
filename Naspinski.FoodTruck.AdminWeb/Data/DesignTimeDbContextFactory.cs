using Naspinski.FoodTruck.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Naspinski.FoodTruck.AdminWeb.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FoodTruckContext>
    {
        public FoodTruckContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<FoodTruckContext>();

            var connectionString = configuration.GetConnectionString("FoodTruckDb");

            builder.UseSqlServer(connectionString);

            return new FoodTruckContext(builder.Options);
        }
    }
}
