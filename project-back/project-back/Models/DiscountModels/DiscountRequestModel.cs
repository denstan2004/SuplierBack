using project_back.Models.Enums;

namespace project_back.Models.DiscountModels
{
    public class DiscountRequestModel
    {
        public int PlannedSales {  get; set; }
        public int DiscountInitPrice{ get; set; }
        public int DiscountPrice { get; set; }
        public int CompensationAmount { get; set;}
        public string Number_ {  get; set; }
        public string CodeWares { get; set; }   
        public RequestStatus Status { get; set; }
        public string DiscountComment { get; set; }
    }
}
