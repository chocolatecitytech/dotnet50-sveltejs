using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExoticRentals.Api.Contexts;
using ExoticRentals.Api.Entities;
using ExoticRentals.Api.Models;
using ExoticRentals.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExoticRentals.Api.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ExoticRentalDbContext _context;
        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IOptions<JwtOptions> jwtOptions,
            ExoticRentalDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions;
            _context = context;
        }

        public async Task<AuthResult> AuthenticateAsync(AuthModel authModel)
        {
            var result = new AuthResult();

            var user = await _userManager.FindByNameAsync(authModel.UserName);
            if (user == null)
                return result;

            var signInResult = await _signInManager.PasswordSignInAsync(user, authModel.Password, true, false);
            if (!signInResult.Succeeded)
                return result;

            result.Token = await GenerateTokenAsync(user);
            result.RefreshToken = await GenerateRefreshTokenAsync();
            var refreshToken = new RefreshToken()
            {
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                Token = result.RefreshToken
            };
            user.RefreshTokens.Add(refreshToken);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            var result = new AuthResult();
            var user = await _context.Users
                .Include(t => t.RefreshTokens)
                .FirstOrDefaultAsync(t => t.RefreshTokens
                .Any(r => r.Token.Equals(refreshToken)));

            if (user == null)
                return result;

            var existingRefreshToken = user.RefreshTokens.First(t => t.Token.Equals(refreshToken));
            if (!existingRefreshToken.IsActive
                || existingRefreshToken.RevokedOn.HasValue
                || DateTime.UtcNow >= existingRefreshToken.ExpiresOn)
            {
                return result;
            }

            //expire and invalidate old token. 
            existingRefreshToken.IsActive = false;
            existingRefreshToken.RevokedOn = DateTime.UtcNow;

            //issue a new token/refresh token
            var newRefresh = new RefreshToken
            {
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(1)
            };
            newRefresh.Token = await GenerateRefreshTokenAsync();
            user.RefreshTokens.Add(newRefresh);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            result.RefreshToken = newRefresh.Token;
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
        private async Task<string> GenerateRefreshTokenAsync(int byteSize = 64)
        {
            using var cryptoProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[byteSize];
            cryptoProvider.GetBytes(randomBytes);
            return await Task.FromResult(Convert.ToBase64String(randomBytes));
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var user = await _context.Users
                .Include(t => t.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token.Equals(refreshToken)));

            if (user == null)
            {
                return false;
            }

            var existingToken = user.RefreshTokens
                .FirstOrDefault(t => t.Token.Equals(refreshToken));

            if (!existingToken.IsActive || DateTime.UtcNow >= existingToken.ExpiresOn)
            {
                return false;
            }
            existingToken.ExpiresOn = DateTime.UtcNow;
            existingToken.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
