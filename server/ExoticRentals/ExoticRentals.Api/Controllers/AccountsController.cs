using System;
using System.Threading.Tasks;
using ExoticRentals.Api.Models;
using ExoticRentals.Api.Services;
using ExoticRentals.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExoticRentals.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserAuthenticationService _authenticationService;

        public AccountsController(IUserAuthenticationService authenticationSerice)
        {
            _authenticationService = authenticationSerice;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var signInResult = await _authenticationService.AuthenticateAsync(model);
            if (!signInResult.IsAuthenticated)
                return BadRequest("User name and/or password does not match.");

            //SetAuthenticationCookie(signInResult.RefreshToken);
            return Ok(signInResult);
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Headers[AuthSettings.REFRESH_TOKEN_HEADER];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized();

            var result = await _authenticationService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return Unauthorized();

            //SetAuthenticationCookie(result.RefreshToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Headers[AuthSettings.REFRESH_TOKEN_HEADER];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized();
            }
            var result = await _authenticationService.LogoutAsync(refreshToken);
            if (!result)
            {
                return Unauthorized();
            }
            return NoContent();
        }

        [HttpGet]
        [Route("secret")]
        public async Task<IActionResult> Secret()
        {
            return new OkObjectResult(await Task.FromResult("you cannot have my secret"));
        }
        private void SetAuthenticationCookie(string token)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            Response.Cookies.Append(AuthSettings.REFRESH_TOKEN_COOKIE, token, cookieOption);

        }
        private void SetAuthenticationHeader(string token)
        {
            Response.Headers.Add(AuthSettings.REFRESH_TOKEN_HEADER, token);
        }
    }
}