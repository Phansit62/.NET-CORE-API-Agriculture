namespace Agriculture.Models
{
    public class TransferPoint
    {
        public int Id { get; set; }
        public string UsersId { get; set; }

        public string ToUsersId { get; set; }

        public int points { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Users Users { get; set; }
        public Users ToUsers { get; set; }
    }
}
