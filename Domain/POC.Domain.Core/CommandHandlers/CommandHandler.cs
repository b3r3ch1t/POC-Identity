using MediatR;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Commands;
using POC.Domain.Core.Interfaces;
using POC.Domain.Core.Logs;
using POC.Domain.Core.Notifications;
using System;

namespace POC.Domain.Core.CommandHandlers
{
    public class CommandHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;

        private readonly ILoggerInterface _loggerInterface;

        protected CommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            ILoggerInterface loggerInterface)
        {
            _uow = uow;
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
            _loggerInterface = loggerInterface;
        }

        protected void NotifyValidationErrors(Command message)
        {
            foreach (var error in message.ValidationResult.Errors)
            {
                _bus.SendDomainNotification(new DomainNotification(key: message.MessageType, value: error.ErrorMessage));
            }
        }

        protected bool Commit()
        {
            if (_notifications.HasNotifications()) return false;

            try
            {
                if (_uow.Commit()) return true;

            }
            catch (Exception ex)
            {
                _bus.SendDomainNotification(new DomainNotification("Commit", "We had a problem during saving your data."));

                _loggerInterface.Erro(ex);
                return false;
            }

            _bus.SendDomainNotification(new DomainNotification("Commit", "We had a problem during saving your data."));

            return false;

        }
    }
}