using project_back.Helpers;
using project_back.Models.DiscountModels;

namespace project_back.DiscountService
{
    public class DiscountService
    {
       public Status<List<StorageAdressModel>> GetAllMergedDiscounts()
        {
            MSSQL mSSQL = new MSSQL();

            return mSSQL.GetAllAdresses();
        }
        /*   public MargedDiscountModel GetMargedDiscount(string number,string code)
           {
               MSSQL mSSQL= new MSSQL();   
               Oracle oracle = new Oracle();   
               return new MargedDiscountModel(oracle.GetSpecificationByCode(code),mSSQL.GetAdressesByNumber(number),mSSQL.GetDicountPeriodByNumber(number));
           }*/
        public List<MargedDiscountModel> GetAllDiscountRequests(string userName, string passwordClaim)
        {
            List<MargedDiscountModel> margedDiscountModels = new List<MargedDiscountModel>();
            MSSQL mSSQL = new MSSQL();
            Oracle oracle = new Oracle(userName, passwordClaim);
            var requests = oracle.GetAllDiscRequests(false);
            foreach (var request in requests.Data)
            {
                var margedModel = new MargedDiscountModel(oracle.GetSpecificationByCode(request.CodeWares), mSSQL.GetAdressesByNumber(request.Number_).Data,
                    mSSQL.GetDicountPeriodByNumber(request.Number_).Data, request.PlannedSales, request.DiscountInitPrice,
                    request.DiscountPrice, request.CompensationAmount, request.Status, request.DiscountComment);
                if (margedModel.adressModel != null && margedModel.discountPeriods != null && margedModel.suplierPostition!=null)
                    margedDiscountModels.Add(margedModel);

            }
            return margedDiscountModels;
        }
        public List<MargedDiscountModel> GetAllDiscRequestsNoDate(string userName, string passwordClaim)
        {
            List<MargedDiscountModel> margedDiscountModels = new List<MargedDiscountModel>();
            MSSQL mSSQL = new MSSQL();
            Oracle oracle = new Oracle(userName, passwordClaim);
            var requests = oracle.GetAllDiscRequests(true);
            foreach (var request in requests.Data)
            {
                var margedModel = new MargedDiscountModel(oracle.GetSpecificationByCode(request.CodeWares), mSSQL.GetAdressesByNumberNoDate(request.Number_),
                    mSSQL.GetDicountPeriodByNumberNoDate(request.Number_), request.PlannedSales, request.DiscountInitPrice,
                    request.DiscountPrice, request.CompensationAmount, request.Status, request.DiscountComment);
                if (margedModel.adressModel != null && margedModel.discountPeriods != null && margedModel.suplierPostition != null)
                    margedDiscountModels.Add(margedModel);

            }
            return margedDiscountModels;
        }
    }
}
