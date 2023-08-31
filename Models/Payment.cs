namespace Agriculture.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrdersId { get; set; }
        public Orders Orders { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int PaymentMethod { get; set; }

        public int Amount { get; set; }

        public int Status { get; set; }

        public int? CouponsId { get; set; }
        public Coupons? Coupons { get; set; }
        public int TransportationCost { get; set; }

        public string? Image { get; set; }
    }
}
