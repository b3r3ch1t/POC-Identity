using System;
using Microsoft.AspNetCore.Identity;

namespace Poc.Infra.CrossCutting.Identity.Models
{

    public class ApplicationUser : IdentityUser<Guid>
    {
        #region Constants

        public const string TableName = "Users";

        #endregion


        public bool Valid { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }

        public void Delete()
        {
            Valid = false;
        }
    }
}
