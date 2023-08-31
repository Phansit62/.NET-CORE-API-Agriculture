namespace Agriculture.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public int Category { get; set; }
        public int Unit { get; set; }
        public List<Images> Images { get; set; }

        public int Stock { get; set; }
    }
}
