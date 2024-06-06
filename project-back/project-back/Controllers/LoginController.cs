using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using project_back.Models;
using project_back.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace project_back.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

        private List<User> users = new List<User>();

        public LoginController()
        {
            users.Add(new User(Guid.Parse("{608580f4-1d82-4230-af18-25e1fa01ff80}"), "manager", "123456", "manager"));
            users.Add(new User(Guid.Parse("{608580f4-1d82-4219-af18-25e1fa01ff80}"), "suplier", "123456", "suplier"));
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModelVM loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = AuthenticateUser(loginModel.Login, loginModel.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

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

            return Ok(new { Message = "Login successful" });
        }
        [HttpGet]
        [Route("test")]
        [Authorize(AuthenticationSchemes = AuthSchemes)]
        public IActionResult test()
        {
            return Ok();
        }
        [HttpGet]
        [Route("cookies")]
        public IActionResult GetCookies()
        {
            var cookies = Request.Cookies.Select(c => new { c.Key, c.Value }).ToList();
            return Ok(cookies);
        }
        [Route("Forbidden")]
        [HttpGet]
        public IActionResult Forbiden()
        {
            return BadRequest("Upss..");
        }
        private User AuthenticateUser(string login, string password)
        {
            return users.FirstOrDefault(user => user.Password == password && user.Name == login);
        }
    }
}
