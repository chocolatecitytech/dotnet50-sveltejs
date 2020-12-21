using ExoticRentals.Api.Models;
using ExoticRentals.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AccountController : ControllerBase
    {
        private readonly IUserAuthenticationService _authenticationSerice;

        public AccountController(IUserAuthenticationService authenticationSerice)
        {
            _authenticationSerice = authenticationSerice;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var signInResult = await _authenticationSerice.AuthenticateAsync(model);
            if (!signInResult.IsAuthenticated)
                return BadRequest("User name and/or password does not match.");

            return Ok(signInResult);

        }

        [Authorize]
        [HttpGet]
        [Route("Secret")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
       public async Task<IActionResult> Secret()
        {
            return new OkObjectResult(await Task.FromResult("you cannot have my secret"));
        }
    }
}
