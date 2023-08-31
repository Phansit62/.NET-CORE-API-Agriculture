namespace Agriculture.Services.Request
{
    public class ProductReq
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public int Category { get; set; }
        public int Unit { get; set; }
        public IFormFile[]? Upload { get; set; }

        public int Stock { get; set; }
        public int[]? removeImage { get; set; }
    }
}
