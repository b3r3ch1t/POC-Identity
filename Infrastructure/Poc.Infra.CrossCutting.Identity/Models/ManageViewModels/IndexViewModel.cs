using System;
using System.ComponentModel.DataAnnotations;

namespace Poc.Infra.CrossCutting.Identity.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }


    }
}
