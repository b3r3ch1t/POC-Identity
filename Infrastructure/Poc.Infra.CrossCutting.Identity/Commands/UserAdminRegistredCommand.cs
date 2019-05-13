using POC.Domain.Core.Commands;

namespace Poc.Infra.CrossCutting.Identity.Commands
{
    public class UserAdminRegistredCommand : Command
    {
        private string _to;
        private string _subject;
        private string _body;

        public UserAdminRegistredCommand(string to, string subject, string body)
        {
            _to = to;
            _subject = subject;
            _body = body;
        }
        public override bool IsValid()
        {
            return true;
        }
    }
}