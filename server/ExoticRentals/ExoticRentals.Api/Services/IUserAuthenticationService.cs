using System.Threading.Tasks;
using ExoticRentals.Api.Models;

namespace ExoticRentals.Api.Services
{
    public interface IUserAuthenticationService
    {
        Task<AuthResult> AuthenticateAsync(AuthModel authModel);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
