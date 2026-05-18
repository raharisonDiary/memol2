using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecensementApi.Data;
using RecensementApi.Models;
using RecensementApi.DTOs;

namespace RecensementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenagesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/menages (Misy Filtre sy Recherche ho an'ny Dashboard sy ny mpanara-maso)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menage>>> GetMenages(
            [FromQuery] string? region = null,
            [FromQuery] string? codeMenage = null,
            [FromQuery] string? niveauSocio = null,
            [FromQuery] int? agentId = null)
        {
            // Atombohana amin'ny requête tsotra fakana ny rehetra
            var query = _context.Menages.AsQueryable();

            // Filtre araka ny Region
            if (!string.IsNullOrEmpty(region))
            {
                query = query.Where(m => m.Region.ToLower() == region.ToLower());
            }

            // Recherche araka ny CodeMenage (mitady izay misy an'ilay teny nomena)
            if (!string.IsNullOrEmpty(codeMenage))
            {
                query = query.Where(m => m.CodeMenage.Contains(codeMenage));
            }

            // Filtre araka ny Niveau Socio-Économique
            if (!string.IsNullOrEmpty(niveauSocio))
            {
                query = query.Where(m => m.NiveauSocioEconomique.ToLower() == niveauSocio.ToLower());
            }

            // Filtre araka ny Agent nanao azy
            if (agentId.HasValue)
            {
                query = query.Where(m => m.AgentId == agentId.Value);
            }

            return await query.ToListAsync();
        }

        // 2. GET: api/menages/{id} (Maka tokantrano iray manokana)
        [HttpGet("{id}")]
        public async Task<ActionResult<Menage>> GetMenage(int id)
        {
            var menage = await _context.Menages.FindAsync(id);

            if (menage == null)
            {
                return NotFound(new { message = "Tsy hita io tokantrano (Ménage) io." });
            }

            return menage;
        }

        // 3. POST: api/menages (Fampidirana tokantrano vaovao avy amin'ny Mobile/Web)
        [HttpPost]
        public async Task<IActionResult> CreateMenage(CreateMenageDto request)
        {
            // 1. Jerena aloha raha tena misy ilay Agent mampiditra azy
            var agentExiste = await _context.Utilisateurs.AnyAsync(u => u.Id == request.AgentId);
            if (!agentExiste)
            {
                return BadRequest(new { message = "Tsy misy io Agent io ao amin'ny rafitra." });
            }

            // 2. Jerena raha efa misy mampiasa ilay CodeMenage mba tsy hisian'ny doublons (CDC)
            var codeExiste = await _context.Menages.AnyAsync(m => m.CodeMenage == request.CodeMenage);
            if (codeExiste)
            {
                return BadRequest(new { message = "Efa misy mampiasa io Code les Ménages io." });
            }

            // 3. Famoronana ilay Objet Menage vaovao
            var menage = new Menage
            {
                CodeMenage = request.CodeMenage,
                Adresse = request.Adresse,
                Region = request.Region,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                NiveauSocioEconomique = request.NiveauSocioEconomique,
                AgentId = request.AgentId,
                DateRecensement = DateTime.UtcNow // Laharana daty androany ho azy
            };

            _context.Menages.Add(menage);
            await _context.SaveChangesAsync();

            // Mamerina ny statut 202 Created miaraka amin'ny URL fakana an'ity ménage ity mivantana
            return CreatedAtAction(nameof(GetMenage), new { id = menage.Id }, menage);
        }

        // 4. PUT: api/menages/{id} (Fanovana angon-drakitra raha nisy diso teo an-kianja)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenage(int id, CreateMenageDto request)
        {
            var menage = await _context.Menages.FindAsync(id);
            if (menage == null)
            {
                return NotFound(new { message = "Tsy hita io tokantrano io hozatra ovaina." });
            }

            // Jerena raha mbola misy CodeMenage hafa mitovy aminy nefa tsy ity id ity
            var codeExiste = await _context.Menages.AnyAsync(m => m.CodeMenage == request.CodeMenage && m.Id != id);
            if (codeExiste)
            {
                return BadRequest(new { message = "Efa misy mampiasa io Code les Ménages io amin'ny tokantrano hafa." });
            }

            // Fanavaozana ny angona
            menage.CodeMenage = request.CodeMenage;
            menage.Adresse = request.Adresse;
            menage.Region = request.Region;
            menage.Latitude = request.Latitude;
            menage.Longitude = request.Longitude;
            menage.NiveauSocioEconomique = request.NiveauSocioEconomique;
            menage.AgentId = request.AgentId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Menages.AnyAsync(m => m.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Voaova soa aman-tsara ny mombamomba ny tokantrano!", data = menage });
        }

        // 5. DELETE: api/menages/{id} (Famafana tokantrano - ho an'ny Admin ihany matetika)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenage(int id)
        {
            var menage = await _context.Menages.FindAsync(id);
            if (menage == null)
            {
                return NotFound(new { message = "Tsy hita io tokantrano io." });
            }

            _context.Menages.Remove(menage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Voafafa soa aman-tsara ilay tokantrano." });
        }
    }
}