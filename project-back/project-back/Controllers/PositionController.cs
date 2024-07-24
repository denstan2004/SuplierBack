using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_back.Helpers;
using project_back.Models;
using project_back.Models.Enums;
using project_back.ViewModel;
using System.Security.Claims;

namespace project_back.Controllers
{
    [Route("api/Positions")]
    public class PositionController : Controller
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

        public PositionController()
        {
           

        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(AuthenticationSchemes = AuthSchemes,Roles = "Supplier")]
        public Status<List<SuplierPostition>> GetAll()
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim);

            List<SuplierPostition> postitions = new List<SuplierPostition>();
                var status = oracle.GetAllSuplier();
                return status;
        }
     
     
        [HttpPost]
        [Route("/Create/Changes")]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Supplier")]
        public Status CreateRequest([FromBody]PriceChangeRequestVM requestsVM)
        {

            bool isAllPased = true;
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim);
            if (requestsVM != null)
            {
                if (requestsVM.ProductUpdateDate > DateTime.Now.AddDays(7))
                {
                    foreach (var position in requestsVM.Supliers)
                    {
                        if (position.status == RequestStatus.Accepted &&  position.IsExpired == false)
                        {
                            isAllPased = false;
                            continue;
                        }
                    
                        PriceChangeRequest request = new PriceChangeRequest(position.CodeFirm, requestsVM.ProductUpdateDate, position.CodeWares, position.NewPrice, RequestStatus.Pennding, "");
                    var status = oracle.CreateRequest(request);

                        if (status.status == false) isAllPased = false;

                    }
                    if (isAllPased == false)
                    {
                        return new Status(-1,"Не всі товари були додані (певні позиції вже були прийняті і термін початку поставок не наступив)");
                    }
                    else
                    {
                        return new Status(0, "Все успішно додано");
                    }
                }
                return new Status(-1,"Некоректна дата");

            }
            return new Status(-1,"Некоректні дані");
        }
       
    }
}
