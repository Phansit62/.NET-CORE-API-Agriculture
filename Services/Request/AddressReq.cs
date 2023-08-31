namespace Agriculture.Services.Request
{
    public class AddressReq
    {
        public int? Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }
        public string? AddressAt { get; set; }

        public int District { get; set; }
        public int Subdistrict { get; set; }
        public int Province { get; set; }

        public string Postcode { get; set; }

        public int Is_default { get; set; }
    }
}
