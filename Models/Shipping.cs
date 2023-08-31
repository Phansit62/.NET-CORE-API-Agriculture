namespace Agriculture.Models
{
    public class Shipping
    {
        public int Id { get; set; }
        public int OrdersId { get; set; }
        public Orders Orders { get; set; }
        public DateTime? ShippingDate { get; set; }

        public int ShoppingType { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
