using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Poc.CrossCutting.Bus;
using Poc.CrossCutting.Serilog;
using Poc.Data.Context;
using Poc.Data.UoW.Equinox.Infra.Data.UoW;
using Poc.Infra.CrossCutting.Identity.Authorization;
using Poc.Infra.CrossCutting.Identity.Data;
using Poc.Infra.CrossCutting.Identity.Repository;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;

namespace Poc.Infra.CrossCuting.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:poc"];

            var serilogDsn = configuration["Sentry:Dsn"];


            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            services.AddScoped<JwtTokenOptions>();


            #region Log

            services.AddScoped<ILoggerInterface>(x => new LoggerInterface(connectionStringMySql: connectionString, serilogDsn: serilogDsn));

            #endregion

            #region  Infra - Data


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<PocContext>();

            #endregion


            services.AddScoped<IUserIdentityRepository, UserIdentityRepository>();



        }
    }
}