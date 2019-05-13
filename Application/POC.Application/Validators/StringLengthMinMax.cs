using System.ComponentModel.DataAnnotations;

namespace POC.Application.Validators
{

    public class StringLengthValidator : ValidationAttribute
    {
        public int MinStringLength { get; set; }
        public int MaxStringLength { get; set; }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return false;
            }
            var length = value.ToString().Length;

            if (length < MinStringLength || length >= MaxStringLength)
            {
                return false;
            }
            return true;
        }
    }

}
