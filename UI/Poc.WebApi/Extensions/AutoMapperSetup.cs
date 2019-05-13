using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using POC.Application.AutoMapper;
using System;

namespace Poc.WebApi.Extensions
{
    public static class AutoMapperSetup
    {

        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper();

            // Registering Mappings automatically only works if the 
            // Automapper Profile classes are in ASP.NET project
            AutoMapperConfig.RegisterMappings();
        }

    }
}
