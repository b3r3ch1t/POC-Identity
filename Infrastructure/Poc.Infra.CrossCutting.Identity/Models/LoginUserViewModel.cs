using System.ComponentModel.DataAnnotations;

namespace Poc.Infra.CrossCutting.Identity.Models
{
    public class LoginUserViewModel
    {


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}
