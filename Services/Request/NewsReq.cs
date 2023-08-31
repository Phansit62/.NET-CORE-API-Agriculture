namespace Agriculture.Services.Request
{
    public class NewsReq
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public IFormFile? Upload { get; set; }
    }
}
