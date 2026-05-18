using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecensementApi.Data;
using RecensementApi.DTOs;

namespace RecensementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatistiquesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatistiquesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/statistiques/global
        [HttpGet("global")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GlobalStatsResponseDto))]
        public async Task<ActionResult<GlobalStatsResponseDto>> GetGlobalStats()
        {
            var totalCitoyens = await _context.Citoyens.CountAsync();
            var totalMenages = await _context.Menages.CountAsync();
            var totalAgents = await _context.Utilisateurs.CountAsync(u => u.Role == "Agent");

            var parSexe = await _context.Citoyens
                .GroupBy(c => c.Sexe)
                .Select(g => new StatRepartionDto { Cle = g.Key, Isany = g.Count() })
                .ToListAsync();

            var parNiveauSocio = await _context.Menages
                .GroupBy(m => m.NiveauSocioEconomique)
                .Select(g => new StatRepartionDto { Cle = g.Key, Isany = g.Count() })
                .ToListAsync();

            var response = new GlobalStatsResponseDto
            {
                TotalMponina = totalCitoyens,
                TotalTokantrano = totalMenages,
                TotalAgentsRecenseurs = totalAgents,
                FitsinjaranaSexe = parSexe,
                FitsinjaranaNiveauSocio = parNiveauSocio
            };

            return Ok(response);
        }

        // 2. GET: api/statistiques/region/{nomRegion}
        [HttpGet("region/{nomRegion}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegionStatsResponseDto))]
        public async Task<ActionResult<RegionStatsResponseDto>> GetRegionStats(string nomRegion)
        {
            var menagesRegion = _context.Menages.Where(m => m.Region.ToLower() == nomRegion.ToLower());
            var totalMenages = await menagesRegion.CountAsync();

            var menageIds = await menagesRegion.Select(m => m.Id).ToListAsync();

            var totalCitoyens = await _context.Citoyens.CountAsync(c => menageIds.Contains(c.MenageId));

            var parSexe = await _context.Citoyens
                .Where(c => menageIds.Contains(c.MenageId))
                .GroupBy(c => c.Sexe)
                .Select(g => new StatRepartionDto { Cle = g.Key, Isany = g.Count() })
                .ToListAsync();

            var response = new RegionStatsResponseDto
            {
                Region = nomRegion,
                TotalMponinaRegion = totalCitoyens,
                TotalTokantranoRegion = totalMenages,
                FitsinjaranaSexeRegion = parSexe
            };

            return Ok(response);
        }

        // 3. GET: api/statistiques/evolution
        [HttpGet("evolution")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EvolutionRecensementDto>))]
        public async Task<ActionResult<IEnumerable<EvolutionRecensementDto>>> GetEvolutionRecensement()
        {
            var evolution = await _context.Menages
                .GroupBy(m => m.DateRecensement.Date)
                .Select(g => new EvolutionRecensementDto
                { 
                    Daty = g.Key.ToString("yyyy-MM-dd"), 
                    IsanTokantranoNampidirina = g.Count() 
                })
                .OrderBy(g => g.Daty)
                .Take(30)
                .ToListAsync();

            return Ok(evolution);
        }
    }
}