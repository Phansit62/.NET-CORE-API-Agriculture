namespace Agriculture.Services.Request
{
    public class UpdateUserReq
    {
        public string? Id { get; set; }
        public int Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile? Upload { get; set; }
    }
}
