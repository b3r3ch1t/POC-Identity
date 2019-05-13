using System;
using System.ComponentModel.DataAnnotations;
using Poc.Infra.CrossCutting.Identity.Validators;

namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class AddClaimUserView
    {
        [Required]
        [GuidValid]
        [Display(Name = "UserId")]
        public Guid UserId { get; set; }

        [Required]
        [Display(Name = "ClaimType")]
        public string ClaimType { get; set; }


        [Required]
        [Display(Name = "ClaimValue")]
        public string ClaimValue { get; set; }

    }
}
