using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorCookie.Controlles
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        public AuthController()
        {

        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody]LoginArgs loginArgs)
        {
            try
            {
                // credential validation ...

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Alex")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = Request.Host.Value
                };

                await AuthenticationHttpContextExtensions.SignInAsync(
                   HttpContext,
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("signOut")]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }
    }    
}
