namespace LoginApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Phone { get; set; }
        public bool EmailConfirmed { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? LastPasswordChangeDate { get; set; } 
        public string? TokenConfirmeEmail { get; set; }
    }

}
