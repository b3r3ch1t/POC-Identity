using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POC.Domain.Core;
using POC.Domain.Core.Bus;
using POC.Domain.Core.ErrorMessages;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System;
using System.Collections.Generic;

namespace Poc.WebApi.Controllers
{
    [AllowAnonymous]
    public class Error : ApiController
    {
        private readonly IMediatorHandler _mediator;
        public Error(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator,
            ILoggerInterface logger) : base(
            notifications,
            mediator,
            logger)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("Error/500")]
        public IActionResult AppError()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            Logger.Erro(exceptionHandlerPathFeature.Error);


            return Ok(
                new { error = "503", success = false, data = "Opss something is wrong!!! " }
            );

        }



        [HttpGet("Error/{statusCode}")]
        public Result<object, IErrorMessage> Error404(int statusCode)
        {

            switch (statusCode)
            {
                case 401:
                    return new Result<object, IErrorMessage>()
                    { Success = true, Errors = new List<IErrorMessage> { new NotFoundError("Id unauthorized!") } };

                case 404:
                    {
                        var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                        Logger.Erro(new Exception("Page not found :" + reExecute.OriginalPath));

                        return new Result<object, IErrorMessage>()
                        { Success = true, Errors = new List<IErrorMessage> { new NotFoundError("Page not found!") } };

                    }

            }


            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature == null)
                return Response("Opss something is wrong!!! ");


            Logger.Erro(exceptionHandlerPathFeature.Error);

            _mediator.SendDomainNotification(new DomainNotification(key: "500", value: exceptionHandlerPathFeature.Error.Message));
            return Response("Opss something is wrong!!! ");


        }
    }
}
