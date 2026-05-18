namespace RecensementApi.DTOs
{
    public class CreateCitoyenDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenoms { get; set; } = string.Empty;
        public string Sexe { get; set; } = string.Empty; // Masculin / Féminin
        public DateTime DateNaissance { get; set; }
        public string? PhotoUrl { get; set; }
        public int MenageId { get; set; } // Ny tokantrano misy azy
        public bool EstChefDeMenage { get; set; } // Famaritana raha izy no loham-by
    }
}