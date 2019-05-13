using System.Net;

namespace POC.Domain.Core.ErrorMessages
{
    public class ValidationError : ErrorMessage
    {
        public ValidationError()
            : base(422, HttpStatusCode.UnprocessableEntity.ToString())
        {
        }


        public ValidationError(string message)
            : base(422, HttpStatusCode.UnprocessableEntity.ToString(), message)
        {
        }
    }
}