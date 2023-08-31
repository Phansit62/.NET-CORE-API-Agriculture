namespace Agriculture.Services.Request
{
    public class CouponReq
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Discount { get; set; }
        public int Points { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public int? limit { get; set; }
    }
}
