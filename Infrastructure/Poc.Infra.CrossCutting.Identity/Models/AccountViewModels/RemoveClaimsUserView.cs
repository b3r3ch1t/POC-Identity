using System.ComponentModel.DataAnnotations;
using Poc.Infra.CrossCutting.Identity.Validators;

namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class RemoveClaimsUserView
    {
        [GuidValid]
        [Required]
        public string Id { get; set; }
        [Required]
        public string ClaimType { get; set; }
        [Required]
        public string ClaimValue { get; set; }
    }
}
