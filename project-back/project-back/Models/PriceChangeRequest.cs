namespace project_back.Models
{
    public class PriceChangeRequest
    {
        public List<SuplierPostition> Supliers { get; set; }
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ProductUpdateDate { get; set; }
        public string Status { get; set; }
        
    }
}
