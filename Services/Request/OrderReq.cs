namespace Agriculture.Services.Request
{
    public class OrderReq
    {
        public string UserId { get; set; }

        public int AddressId { get; set; }

        public int PaymentMethod { get; set; }

        public int? CouponId { get; set; }

        public int TotalPrice { get; set; }

        public IFormFile? Upload { get; set; }

        public List<Items>? Products { get; set; }

    }
    public class Items
    {
        public int ProductId { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }
    }
}
