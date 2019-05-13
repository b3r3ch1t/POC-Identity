using POC.Domain.Core.Commands;
using System;

namespace Poc.Infra.CrossCutting.Identity.Commands
{
    public class RegistrationConfirmedCommand : Command
    {
        private string _to;
        private string _subject;
        private string _body;


        public RegistrationConfirmedCommand(string to, string subject, string body)
        {
            _to = to;
            _subject = subject;
            _body = body;
        }
        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
