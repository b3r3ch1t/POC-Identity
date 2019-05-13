using POC.Domain.Core.Commands;
using POC.Domain.Core.Events;
using POC.Domain.Core.Notifications;
using System.Threading.Tasks;

namespace POC.Domain.Core.Bus
{
    public interface IMediatorHandler
    {
        Task SendCommandAsync<T>(T command) where T : Command;
        Task RaiseEventAsync<T>(T message) where T : Event;

        Task SendDomainNotification(DomainNotification message);
    }
}
