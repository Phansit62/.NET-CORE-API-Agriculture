namespace Agriculture.Models
{
    public class CouponUser
    {
        public int Id { get; set; }
        public int CouponsId { get; set; }
        public Coupons Coupons { get; set; }
        
        public string UsersId { get; set; }

        public Users Users { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? IsUsed { get; set; }
    }
}
