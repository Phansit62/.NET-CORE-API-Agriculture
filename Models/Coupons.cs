namespace Agriculture.Models
{
    public class Coupons
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Discount { get; set; }

        public int Points { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }

        public int? limit { get; set; }
    }
}
