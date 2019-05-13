using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POC.Domain.Core;
using POC.Domain.Core.Bus;
using POC.Domain.Core.ErrorMessages;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System.Collections.Generic;
using System.Linq;

namespace Poc.WebApi.Controllers
{
    public abstract class ApiController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _mediator;
        protected readonly ILoggerInterface Logger;


        protected ApiController(
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator,
            ILoggerInterface logger)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediator = mediator;
            Logger = logger;
        }

        protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();

        private bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());

        }

        protected new Result<object, IErrorMessage> Response(object data = null)
        {
            if (IsValidOperation())
            {
                return new Result<object, IErrorMessage>()
                { Success = true, Errors = new List<IErrorMessage>(), Data = data };
            }

            var result = new Result<object, IErrorMessage> { Success = false };



            var listErros = _notifications.GetNotifications();

            foreach (var error in listErros)
            {
                result.Errors.Add(new ValidationError(message: error.Value));
            }



            return result;
        }

        protected void NotifyModelStateErrors()
        {
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotifyError(string.Empty, erroMsg);
            }
        }

        protected void NotifyError(string code, string message)
        {
            _notifications.AddNotifications(key: code, message);
        }
        protected void AddIdentityErrors(IdentityResult result)
        {

            foreach (var error in result.Errors)
            {
                _notifications.AddNotifications(key: error.Code, error.Description);
            }
        }
    }
}
