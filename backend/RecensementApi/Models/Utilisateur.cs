using System.ComponentModel.DataAnnotations;

namespace RecensementApi.Models
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Agent"; // Admin, Responsable Regional, Agent

        public string? RegionAffectation { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}