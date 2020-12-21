using ExoticRentals.Api.Entities;
using ExoticRentals.Api.Settings;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.SeedData
{
    public static class SeedData
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Data

            if (roleManager.Roles.Count() == 0)
            {
                await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Manager.ToString()));
                await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Staff.ToString()));
                await roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Customer.ToString()));
            }

            var testUser = new ApplicationUser
            {
                FirstName = "Test User",
                LastName = "Last Test",
                Email = "TestUser@chocolatecitytech.com",
                UserName = "TestUser",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Id = Guid.NewGuid().ToString(),

            };
            if(userManager.Users.Count() == 0)
            {
                await userManager.CreateAsync(testUser, "Secret123%#");
                await userManager.AddToRoleAsync(testUser, ApplicationRoles.Staff.ToString());
            }
        }
    }
}
