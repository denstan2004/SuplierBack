using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using project_back.Models;
using project_back.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using project_back.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace project_back.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

      

        [HttpPost]
        public async Task<Status<UserRolesOracle>> LoginAsync([FromBody] LoginModelVM loginModel)
            {
            if (!ModelState.IsValid)
            {
                return new Status<UserRolesOracle>(-1,"Некоректні данні");
            }

            Oracle oracle = new Oracle(loginModel.Login, loginModel.Password);
            Status<UserRolesOracle> status = oracle.GetRole(loginModel.Login, loginModel.Password);
            

            if (status.Data == null ||(status.Data.IsSupplier == false && status.Data.IsManager == false))
            {
                return new Status<UserRolesOracle>(System.Net.HttpStatusCode.Unauthorized);
            }
            
            var role = status.Data;
            
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, loginModel.Login),
        new Claim("Password", loginModel.Password) // Додаємо пароль до claims
    };

            if (role.IsManager == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Manager"));
            }
            else if (role.IsSupplier == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Supplier"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Залишаємо кукі після закриття браузера
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return status;
        }


        [HttpGet]
        [Route("test")]        
        public Status  CheckAuthorization()
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim);
            Status<UserRolesOracle> status = oracle.GetRole(userName, passwordClaim);
            return status;
        }
        [HttpGet]
        [Authorize]
        public void IsCookieValid()
        {
            
        }
    }
}
