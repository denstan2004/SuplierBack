using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_back.Helpers;
using project_back.Models;
using project_back.ViewModel;

namespace project_back.Controllers
{
    [Route("api/Request")]
    public class RequestController: Controller
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

        [HttpGet]
        [Route("GetAll")]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Manager")]
        public Status<List<SuplierPostition>> GetAllForSuplier()
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim); 
            return oracle.GetAllSpecificationManager();
          
        }
        [HttpPost]
        [Route ("Update")]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Manager")]

        public Status UpdateRequest([FromBody] ChangeRequestStatus change)
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim); 
            return oracle.UpdateRequest(change);
        }
    }
}
