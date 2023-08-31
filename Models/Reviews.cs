namespace Agriculture.Models
{
    public class Reviews
    {
        public int Id { get; set; }

        public int ProductsId { get; set; }
        public Products Products { get; set; }
         
        public DateTime? CreatedAt { get; set; }

        public string? Comment { get; set; }
         
        public int rating { get; set; }

        public string UsersId { get; set; }
        public Users Users { get; set; }
    }
}
