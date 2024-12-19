namespace LoginApp.Dto
{
    public class LoginDto(string Email, string Password)
    {
        public string Email { get; set; } = Email;
        public string Password { get; set; } = Password;
    }

    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }        
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Phone { get; set; }
    }

    public class ForgotPasswordDto
    {
        public required string Email { get; set; }
    }
}
