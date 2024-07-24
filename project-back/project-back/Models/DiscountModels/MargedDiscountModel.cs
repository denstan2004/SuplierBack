using project_back.Models.Enums;

namespace project_back.Models.DiscountModels
{
    public class MargedDiscountModel
    {
      public SuplierPostition suplierPostition {  get; set; } 
      public StorageAdressModel adressModel {  get; set; }
      public DiscountPeriodsModel discountPeriods { get; set; }

        public int PlannedSales { get; set; }
        public int DiscountInitPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int CompensationAmount { get; set; }
        public RequestStatus Status { get; set; }   
        public string DiscountComment { get; set; }

        public MargedDiscountModel(SuplierPostition suplierPostition, StorageAdressModel adressModel, DiscountPeriodsModel discountPeriods, int plannedSales, int disountInitPrice, int disountPrice, int compensationAmount, RequestStatus status, string discountComment) : this(suplierPostition, adressModel, discountPeriods)
        {
            PlannedSales = plannedSales;
            DiscountInitPrice = disountInitPrice;
            DiscountPrice = disountPrice;
            CompensationAmount = compensationAmount;
            Status = status;
            DiscountComment = discountComment;
        }

        public MargedDiscountModel(SuplierPostition suplierPostition, StorageAdressModel adressModel, DiscountPeriodsModel discountPeriods)
        {
            this.suplierPostition = suplierPostition;
            this.adressModel = adressModel;
            this.discountPeriods = discountPeriods;
        }
    }
}
