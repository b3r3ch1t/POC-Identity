using POC.Domain.Core.Interfaces;

namespace POC.Domain.Core.ErrorMessages
{
    public abstract class ErrorMessage : IErrorMessage
    {
        public int StatusCode { get; }

        public string StatusDescription { get; }

        public string Message { get; }

        protected ErrorMessage(int statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public ErrorMessage(int statusCode, string statusDescription, string message)
            : this(statusCode, statusDescription)
        {
            Message = message;
        }
    }
}
