namespace project_back.Models
{
    public class SuplierPostition
    {
        private List<string> _barCodes;

        public SuplierPostition()
        {
        }

        public SuplierPostition(string nameBrand, string barCode, int code, double price, string aritcle, int codeWares, string nameWares, int brandCode)
        {
            NameBrand = nameBrand;
            BarCode = barCode;
            Code = code;
            Price = price;
            Aritcle = aritcle;
            CodeWares = codeWares;
            NameWares = nameWares;
            BrandCode = brandCode;
            _barCodes = barCode?.Split(';').ToList() ?? new List<string>();
        }

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

        public string NameBrand { get; set; }
        public string BarCode { get; set; }
        public int Code { get; set; }
        public double Price { get; set; }
        public string Aritcle { get; set; }
        public int CodeWares { get; set; }
        public string NameWares { get; set; }
        public int BrandCode { get; set; }
    }
}
