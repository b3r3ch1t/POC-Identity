using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Poc.Infra.CrossCutting.Identity.Models;
using POC.Domain.Core.Extensions;

namespace Poc.Infra.CrossCutting.Identity.Data
{
    public class ApplicationDataContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {

        #region Constructor 

        public ApplicationDataContext(
            DbContextOptions<ApplicationDataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);




            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var table = entityType.Relational().TableName;
                if (table.StartsWith("AspNet"))
                {
                    entityType.Relational().TableName = "hd_" + table.Substring(6).ToLower();

                }
            }


            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.Property(au => au.Id)
                    .HasColumnName("role_id");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.Property(au => au.Id)
                    .HasColumnName("userclaim_id");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.Property(au => au.RoleId)
                    .HasColumnName("userrole_id");

                b.Property(au => au.UserId)
                    .HasColumnName("userId_id");
            });


            modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.Property(au => au.UserId)
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.Property(au => au.Id)
                    .HasColumnName("roleclaim_id");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.Property(au => au.UserId)
                    .HasColumnName("usertoken_id");
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(au => au.Id)
                    .HasColumnName("user_id");
            });

        }


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


        public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
        {
            public ApplicationDataContext CreateDbContext(string[] args)
            {

                Debugger.Launch();

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


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


}
