using System;

namespace ExoticRentals.Api.Entities
{
    public class RefreshToken
    {
        public Guid Key { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive { get; set; }
        public ApplicationUser User { get; set; }
    }
}
