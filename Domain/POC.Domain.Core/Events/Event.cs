using MediatR;
using POC.Domain.Core.Messages;
using System;

namespace POC.Domain.Core.Events
{
    public abstract class Event : Message, INotification
    {
        public DateTime Timestamp { get; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
