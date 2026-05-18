using System.ComponentModel.DataAnnotations;

namespace RecensementApi.Models
{
    public class Menage
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string CodeMenage { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Adresse { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [StringLength(50)]
        public string NiveauSocioEconomique { get; set; } = "Moyen";

        public int AgentId { get; set; }
        public Utilisateur? Agent { get; set; }

        public DateTime DateRecensement { get; set; } = DateTime.UtcNow;

        public ICollection<Citoyen> Membres { get; set; } = new List<Citoyen>();
    }
}