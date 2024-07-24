using project_back.Models.Enums;
using System.Text.Json.Serialization;

namespace project_back.Models
{
    public class PriceChangeRequest //модель для збереження в бд
    {
        public DateTime DateSpecification { get; set; }
        public string CodeFirm { get; set; }
        public DateTime DateStart { get; set; }
        public string CodeWares { get; set; }
        public double Price { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestStatus status {get;set;} 
        public string CommentSpec { get; set;}

        public PriceChangeRequest()
        {
        }

        public PriceChangeRequest( string codeFirm, DateTime dateStart, string codeWares, double price, RequestStatus status, string comment)
        {
            CodeFirm = codeFirm;
            DateStart = dateStart;
            CodeWares = codeWares;
            Price = price;
            this.status = status;
            CommentSpec = comment;
        }
    }
}
