using System.Text.Json.Serialization;

namespace Agriculture.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrdersId { get; set; }
        [JsonIgnore]
        public Orders Orders { get; set;}

        public int ProductsId { get; set; }
        public Products Products { get; set;}

        public int Quantity { get; set; }

        public int? TotalPrice { get; set; }
    }
}
