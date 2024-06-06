namespace project_back.Models
{
    public class SuplierPostition
    {
        public Guid Id { get; set; }
        public Guid SuplierId { get; set; }
        public string Trademark { get; set; }
        public double Price { get; set; }
        public string Aritcle { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }

        public SuplierPostition(Guid id, Guid suplierId, string trademark, double price, string aritcle, string positionCode, string positionName)
        {
            Id = id;
            SuplierId = suplierId;
            Trademark = trademark;
            Price = price;
            Aritcle = aritcle;
            PositionCode = positionCode;
            PositionName = positionName;
        }
    }
}
