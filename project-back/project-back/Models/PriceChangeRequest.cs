namespace project_back.Models
{
    public class PriceChangeRequest
    {
        Guid Id { get; set; }
        Guid ProductId { get; set; }
        Guid SuplierId { get; set; }
        string PositionCode { get; set; }
        string PositionName { get; set; }
        string article { get; set; }
        int Price { get; set; }
        string TradeMark { get; set; }
        int NewPrice { get; set; }
        string Status {  get; set; }
        string Comment {  get; set; }
        DateTime CreatiomTime { get; set; }
    }
}
