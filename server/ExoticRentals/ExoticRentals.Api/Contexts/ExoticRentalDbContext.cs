using ExoticRentals.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExoticRentals.Api.Contexts
{
    public class ExoticRentalDbContext : IdentityDbContext<ApplicationUser>
    {
        public ExoticRentalDbContext(DbContextOptions<ExoticRentalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ExoticRentalDbContext).Assembly);
        }
    }
}
