using System.ComponentModel.DataAnnotations;

namespace RecensementApi.Models
{
    public class Citoyen
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Prenoms { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string Sexe { get; set; } = string.Empty;

        [Required]
        public DateTime DateNaissance { get; set; }

        public string? PhotoUrl { get; set; }

        [Required]
        public int MenageId { get; set; }
        public Menage? Menage { get; set; }

        public bool EstChefDeMenage { get; set; } = false;
    }
}