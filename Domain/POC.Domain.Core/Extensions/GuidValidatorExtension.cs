using POC.Domain.Core.Validators;

namespace POC.Domain.Core.Extensions
{
    public static class GuidValidatorExtension
    {

        public static bool IsValidGuid(this string guidCandidate)
        {
            var result = new GuidValidator().Validate(guidCandidate).IsValid;

            return result;
        }
    }
}
