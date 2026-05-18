namespace RecensementApi.DTOs
{
    public class RegisterDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Agent"; // Agent, ResponsableRegional, Admin
        public string? RegionAffectation { get; set; }
    }
}