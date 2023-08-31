namespace Agriculture.Services.Request
{
    public class ReviewsReq
    {
        public int Id { get; set; }

        public int ProductsId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Comment { get; set; }

        public int rating { get; set; }

        public string UsersId { get; set; }
        
        public int OrderId { get; set; }
    }
}
