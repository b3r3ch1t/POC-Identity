using System;
using System.ComponentModel.DataAnnotations;

namespace Poc.Infra.CrossCutting.Identity.Validators
{
    public class GuidValidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var guidValid = new Guid(value.ToString());
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(" Este código não é um GUID válido !!!!");
            }
        }
    }
}
