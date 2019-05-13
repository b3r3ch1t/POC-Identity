using System.ComponentModel.DataAnnotations;

namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
