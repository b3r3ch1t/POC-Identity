using System;
using System.ComponentModel.DataAnnotations;
using Poc.Infra.CrossCutting.Identity.Validators;

namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class UpdateViewModel
    {

        [Required]
        [GuidValid]
        [Display(Name = "UserId")]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "ProfilePicture")]
        public string ProfilePicture { get; set; }

    }
}
