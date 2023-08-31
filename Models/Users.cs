namespace Agriculture.Models
{
    public class Users
    {
        public string? Id { get; set; }
        public int Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int? Points { get; set; }
        public DateTime? Create_At { get; set; }
        public int? Role { get; set; }

        public string? ImageProfile { get; set; }

    }
}
