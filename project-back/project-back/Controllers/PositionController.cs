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
            supliersPositions = new List<SuplierPostition>();
            double s= 29.50;
            
            supliersPositions.Add(new SuplierPostition(Guid.NewGuid(), Guid.Parse("{608580f4-1d82-4219-af18-25e1fa01ff80}"), "Coca-Cola", s, "article1", "1234567890123", "name1"));
            supliersPositions.Add(new SuplierPostition(Guid.NewGuid(), Guid.Parse("{608580f4-1d82-4219-af18-25e1fa01ff80}"), "Чумак", s, "article2", "1234567890124", "name2"));

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
                    var suplierPositions = GetAllByUser(Guid.Parse(userIdClaim.Value));
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
        private List<SuplierPostition> GetAllByUser(Guid UserId)
        {
            return supliersPositions.Where(position => position.SuplierId == UserId).ToList();

        }
    }
}
