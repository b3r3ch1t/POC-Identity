using System;
using System.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Poc.Infra.CrossCutting.Identity.Data;

namespace Poc.Infra.CrossCutting.Identity.Factory
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
    {
        public ApplicationDataContext CreateDbContext(string[] args)
        {

            Debugger.Launch();

            // Get environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Build config
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var connectionString = config["ConnectionStrings:poc"];

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();

            optionsBuilder.UseMySql(connectionString);

            return new ApplicationDataContext(optionsBuilder.Options);

        }

    }
}
