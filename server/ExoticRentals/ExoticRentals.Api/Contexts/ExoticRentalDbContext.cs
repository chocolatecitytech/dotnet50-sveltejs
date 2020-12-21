using ExoticRentals.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Contexts
{
    public class ExoticRentalDbContext:IdentityDbContext<ApplicationUser>
    {
        public ExoticRentalDbContext(DbContextOptions<ExoticRentalDbContext> options)
            :base(options)
        {           
        }
    }
}
