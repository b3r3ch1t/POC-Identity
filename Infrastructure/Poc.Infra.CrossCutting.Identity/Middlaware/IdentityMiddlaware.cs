using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poc.Infra.CrossCutting.Identity.Data;
using Poc.Infra.CrossCutting.Identity.Models;

namespace Poc.Infra.CrossCutting.Identity.Middlaware
{
    public static class IdentityMiddlaware
    {
        public static void ConfigureIdentity(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDataContext>(options =>
                options.UseMySql(connectionString));


            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDataContext>()
                .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8; //Valor default: 6
                options.Password.RequiredUniqueChars = 6; //Valor default = 1
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(6);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });
        }
    }
}
