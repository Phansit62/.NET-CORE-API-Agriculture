using System.Text.Json.Serialization;

namespace Agriculture.Models
{
    public class Images
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int ProductsId { get; set; }

        [JsonIgnore]
        public Products Products { get; set; }
    }
}
