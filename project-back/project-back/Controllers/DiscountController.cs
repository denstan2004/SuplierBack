using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_back.DiscountService;
using project_back.Helpers;
using project_back.Models.DiscountModels;
using project_back.Models.Enums;
using project_back.ViewModel;

namespace project_back.Controllers
{
    [Route("api/Discount")]
    public class DiscountController : Controller
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme;

        [Route("Create")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Supplier")]
        public Status Create([FromBody] AddDiscountVM addDiscount)
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim);
            //MSSQL mSSQL = new MSSQL();
          var status=  oracle.AddDiscount(addDiscount);
            return status;
        }
        [Route("GetAllRequests/Suplier")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Supplier")]
        public List<MargedDiscountModel> GetAllRequestsSuplier()
            {
            try
            {
                DiscountService.DiscountService service = new DiscountService.DiscountService();
                var userName = User.Identity?.Name;
                var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
                var response = service.GetAllDiscRequestsNoDate(userName, passwordClaim);
                return response;
            }
            catch (Exception ex)
            {
                return new List<MargedDiscountModel>();
            }
        }
        [Route("GetAll/Discounts/Adress")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Supplier")]

        public Status<List<StorageAdressModel>> GetAllDiscAdress()
        {
            DiscountService.DiscountService service = new DiscountService.DiscountService();
            return service.GetAllMergedDiscounts();
        }
        [Route("GetAll/Discount/Time")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Supplier")]

        public Status<List<DiscountPeriodsModel>> GetAllDiscTime()
        {
            MSSQL mSSQL = new MSSQL();
            return mSSQL.GetAllDiscPeriods();

        }
        [Route("GetAll/Discount/Request")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Manager")]
        public List<MargedDiscountModel> GetAllDiscRequests()
        {
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            DiscountService.DiscountService service = new DiscountService.DiscountService();
            return service.GetAllDiscountRequests(userName, passwordClaim);
        }
        //Supplier
        [Route("Update/status")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Manager")]
        public Status ChangeStatus([FromBody] ChangeDiscountRequestStatus model)
        {
            if (model == null)
            {
                return new Status(-1,"Model is null");
            }
            var userName = User.Identity?.Name;
            var passwordClaim = User.Claims.FirstOrDefault(c => c.Type == "Password")?.Value;
            Oracle oracle = new Oracle(userName, passwordClaim);
            return oracle.UpdateStatus(model.status, model.number, model.comment, model.codewares);
            
        }


    }
}
