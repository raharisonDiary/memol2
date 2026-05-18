namespace RecensementApi.DTOs
{
    public class CreateMenageDto
    {
        public string CodeMenage { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string NiveauSocioEconomique { get; set; } = string.Empty; // Faible, Moyen, Élevé
        public int AgentId { get; set; }
    }
}