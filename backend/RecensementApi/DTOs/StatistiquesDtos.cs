namespace RecensementApi.DTOs
{
    // 1. DTO ho an'ny fitsinjarana tsotra (Sexe na Niveau Socio)
    public class StatRepartionDto
    {
        public string Cle { get; set; } = string.Empty; // Sexe na Niveau
        public int Isany { get; set; }
    }

    // 2. DTO ho an'ny Antontan'isa Ankapobeny (Global)
    public class GlobalStatsResponseDto
    {
        public int TotalMponina { get; set; }
        public int TotalTokantrano { get; set; }
        public int TotalAgentsRecenseurs { get; set; }
        public List<StatRepartionDto> FitsinjaranaSexe { get; set; } = new();
        public List<StatRepartionDto> FitsinjaranaNiveauSocio { get; set; } = new();
    }

    // 3. DTO ho an'ny Antontan'isa isaky ny Faritra
    public class RegionStatsResponseDto
    {
        public string Region { get; set; } = string.Empty;
        public int TotalMponinaRegion { get; set; }
        public int TotalTokantranoRegion { get; set; }
        public List<StatRepartionDto> FitsinjaranaSexeRegion { get; set; } = new();
    }

    // 4. DTO ho an'ny fivoaran'ny asa isan'andro
    public class EvolutionRecensementDto
    {
        public string Daty { get; set; } = string.Empty;
        public int IsanTokantranoNampidirina { get; set; }
    }
}