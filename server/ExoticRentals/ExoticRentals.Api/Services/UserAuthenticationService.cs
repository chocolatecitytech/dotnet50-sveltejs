using ExoticRentals.Api.Entities;
using ExoticRentals.Api.Models;
using ExoticRentals.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtOptions> _jwtOptions;
        public UserAuthenticationService(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions;
        }
        
        public async Task<AuthResult> AuthenticateAsync(AuthModel authModel)
        {
            var result = new AuthResult();

            var user = await _userManager.FindByNameAsync(authModel.UserName);
            if (user == null)
                return result;

            var signInResult = await _signInManager.PasswordSignInAsync(user, authModel.Password,true,false);
            if (!signInResult.Succeeded)
                return result;

            result.Token = await GenerateTokenAsync(user);

            return result;
        }
        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var token = string.Empty;
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = userClaims.ToList();
            claims.AddRange(userRoles.Select(u => new Claim(ClaimTypes.Role, u)));
            claims.AddRange(new Claim[]
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Sub, user.Email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString())

            });

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions?.Value?.Key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                issuer: _jwtOptions?.Value?.Issuer,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.Value.Expiry),
                audience: _jwtOptions?.Value?.Audience
                );
            token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            
            return await Task.FromResult(token);
        }
    }
}
