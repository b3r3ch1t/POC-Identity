using System;

namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class ClaimViewModel
    {

        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
