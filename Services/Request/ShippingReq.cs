namespace Agriculture.Services.Request
{
    public class ShippingReq
    {
        public int Id { get; set; }
        public int OrdersId { get; set; }
        public int ShoppingType { get; set; }
        public string? TrackingNumber { get; set; }
    }
}
