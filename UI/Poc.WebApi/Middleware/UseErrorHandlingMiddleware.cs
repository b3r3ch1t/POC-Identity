using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Poc.WebApi.Middleware
{
    public class UseErrorHandlingMiddleware
    {
        private static RequestDelegate SQLNext;

        public UseErrorHandlingMiddleware(RequestDelegate next)
        {
            SQLNext = next;

        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await SQLNext(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;


            var logger = context.RequestServices.GetService<ILoggerInterface>();

            var mediator = context.RequestServices.GetService<IMediatorHandler>();

            //var logger = new LoggerInterface(connectionStringMySql: _connectionString, serilogDsn: _serilogDsn);
            mediator.SendDomainNotification(new DomainNotification(key: "500", value: "Message !"));


            logger.Erro(exception);

            return SQLNext(context);

        }


    }
}
