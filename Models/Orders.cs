namespace Agriculture.Models
{
    public class Orders
    {
        public int Id { get; set; }

        public string UsersId { get; set; }
        public Users Users { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? OrderStatus { get; set; }
         
        public int? TotalPrice { get; set; }

        public int? AddressId { get; set; }

        public int? IsReview { get; set; } 
        public Address Address { get; set; }

        public List<OrderDetail> OrderDetail { get; set; }
    }
}
