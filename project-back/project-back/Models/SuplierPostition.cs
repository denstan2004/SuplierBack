using project_back.Models.Enums;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace project_back.Models
{
    public class SuplierPostition
    {

        public SuplierPostition()
        {
        }

        public SuplierPostition(  string codeFirm, string nameBrand, string barCode, string code, double price, string aritcle, string codeWares, string nameWares, string brandCode, double newPrice, RequestStatus status, string commentSpec, DateTime dateSpecification, DateTime dateStart)
        {
       
            CodeFirm = codeFirm;
            NameBrand = nameBrand;
            BarCode = barCode;
            Code = code;
            Price = price;
            Aritcle = aritcle;
            CodeWares = codeWares;
            NameWares = nameWares;
            BrandCode = brandCode;
            NewPrice = newPrice;
            this.status = status;
            CommentSpec = commentSpec;
            DateSpecification = dateSpecification;
            DateStart = dateStart;
        }
        private List<string> _barCodes;

        public List<string> BarCodes
        {
            get
            {
                if (_barCodes == null && !string.IsNullOrEmpty(BarCode))
                {
                    _barCodes = BarCode.Split(';').ToList();
                }
                return _barCodes;
            }
            set
            {
                _barCodes = value;
                BarCode = string.Join(";", _barCodes);
            }
        }
        public bool IsExpired
        {
            get
            {
                return DateTime.Now>= DateStart;
            }
        }
        public double ChangePercent
        {
            get
            {
                if (UpdatedPrice != 0)
                {
                    return Math.Round((UpdatedPrice / Price * 100 - 100)    , 2); 
                }
                return 0;
            }
                    
        }
        public string GroupName { get; set; }
        public string CodeFirm {  get; set; }
        public string NameBrand { get; set; }
        public string BarCode { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public string Aritcle { get; set; }
        public string CodeWares { get; set; }
        public string NameWares { get; set; }
        public string BrandCode { get; set; }
        public double NewPrice { get; set; } // ціна яка прийде з фронту
        public double UpdatedPrice { get; set; } //ціна яка приходить з бд

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestStatus status { get; set; }
        public string CommentSpec { get; set; }
        public DateTime DateSpecification { get; set; }
        public DateTime DateStart { get; set; }
    }
}
