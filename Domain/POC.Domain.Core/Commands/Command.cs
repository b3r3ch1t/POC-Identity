using FluentValidation.Results;
using POC.Domain.Core.Messages;
using System;

namespace POC.Domain.Core.Commands
{
    public abstract class Command : Message
    {
        private DateTime Timestamp { get; }
        public ValidationResult ValidationResult { get; protected set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool IsValid();
    }
}
