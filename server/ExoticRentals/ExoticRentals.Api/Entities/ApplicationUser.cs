using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ExoticRentals.Api.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.RefreshTokens = new List<RefreshToken>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
