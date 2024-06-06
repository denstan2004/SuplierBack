using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_back.Models;
using System.Security.Claims;

namespace project_back.Controllers
{
    [Route("api/Positions")]
    public class PositionController : Controller
    {
        List<SuplierPostition> supliersPositions;
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

        public PositionController()
        {
           

        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(AuthenticationSchemes = AuthSchemes)]
        public IActionResult GetAll()
        {
            List<SuplierPostition> postitions = new List<SuplierPostition>();
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (userIdClaim != null && userRoleClaim.Value=="suplier")
            {
                try
                {
                    var suplierPositions = GetAllByUser();
                    if (suplierPositions != null)
                    {
                        return Ok(suplierPositions);
                    }
                }
                catch (Exception ex) 
                {
                    return BadRequest(ex.Message);
                }

            }
            return BadRequest("User Expired or wrong Role");
        }
        [HttpGet]
        [Route("test/db")]
        public IActionResult Get()
        {
            Oracle oracle = new Oracle();
            var res= oracle.res();
            return Ok(res);
        }
        private List<SuplierPostition> GetAllByUser()
        {
            Oracle oracle = new Oracle();
            return oracle.res();
        }
        [HttpPost]
        [Route("/Create/Changes")]
        public IActionResult CreateRequest([FromBody]PriceChangeRequest requests)
        {
            requests.Status = "pending";

            return Ok();
        }
    }
}
