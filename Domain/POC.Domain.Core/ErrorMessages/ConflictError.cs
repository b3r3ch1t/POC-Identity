using System.Net;

namespace POC.Domain.Core.ErrorMessages
{
    public class ConflictError : ErrorMessage
    {
        public ConflictError()
            : base(409, HttpStatusCode.Conflict.ToString())
        {
        }

        public ConflictError(string message)
            : base(409, HttpStatusCode.UnprocessableEntity.ToString(), message)
        {
        }
        public ConflictError(int statusCode, string message)
            : base(statusCode, HttpStatusCode.UnprocessableEntity.ToString(), message)
        {
        }
    }
}