using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using POC.Domain.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.Data.Context
{
    public class PocContext : DbContext
    {


     #region SaveChanges
        public override int SaveChanges()
        {
            UpdateData();

            return base.SaveChanges();
        }

        private void UpdateData()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(entry => entry.Entity.GetType().GetProperty("DateCreated") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DateCreated").CurrentValue = DateTime.Now;
                    entry.Property("DateUpdated").CurrentValue = DateTime.Now;
                    entry.Property("Valid").CurrentValue = true;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DateCreated").IsModified = false;
                    entry.Property("DateUpdated").CurrentValue = DateTime.Now;
                }

                if (!entry.Property("Id").CurrentValue.ToString().IsValidGuid() || entry.Property("Id").CurrentValue == null ||
                    string.IsNullOrWhiteSpace(entry.Property("Id").CurrentValue.ToString()))
                {
                    entry.Property("Id").CurrentValue = Guid.NewGuid();
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateData();


            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            var connectionString = config["ConnectionStrings:poc"];

            optionsBuilder.UseMySql(connectionString);

        }
    }
}
