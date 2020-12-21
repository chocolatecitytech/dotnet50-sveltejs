using ExoticRentals.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Services
{
    public interface IUserAuthenticationService
    {
        Task<AuthResult> AuthenticateAsync(AuthModel authModel);
    }
}
