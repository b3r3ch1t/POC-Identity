using MediatR;
using POC.Domain.Core.Bus;
using POC.Domain.Core.Commands;
using POC.Domain.Core.Events;
using POC.Domain.Core.Messages;
using POC.Domain.Core.Notifications;
using System.Threading.Tasks;

namespace Poc.CrossCutting.Bus
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Task PublishAsync<T>(T mensagem) where T : Message
        {
            var notification = (INotification)mensagem;
            return _mediator.Publish(notification);
        }

        public Task SendCommandAsync<T>(T command) where T : Command
        {
            return PublishAsync(command);
        }


        public Task RaiseEventAsync<T>(T message) where T : Event
        {
            return PublishAsync(message);
        }

        public Task SendDomainNotification(DomainNotification message)
        {
            return _mediator.Publish(message);
        }
    }
}
