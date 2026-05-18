using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecensementApi.Data;
using RecensementApi.Models;
using RecensementApi.DTOs;

namespace RecensementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitoyensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitoyensController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/citoyens (Misy Filtre sy Recherche)
        // Ohatra: api/citoyens?sexe=Masculin&nom=Raharison
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citoyen>>> GetCitoyens(
            [FromQuery] string? nom = null,
            [FromQuery] string? sexe = null,
            [FromQuery] int? menageId = null)
        {
            var query = _context.Citoyens.AsQueryable();

            // Fikarohana amin'ny Anarana na Fanampiny
            if (!string.IsNullOrEmpty(nom))
            {
                query = query.Where(c => c.Nom.Contains(nom) || c.Prenoms.Contains(nom));
            }

            // Filtre araka ny Sexe
            if (!string.IsNullOrEmpty(sexe))
            {
                query = query.Where(c => c.Sexe.ToLower() == sexe.ToLower());
            }

            // Filtre mampiseho ny olona ao anatin'ny tokantrano iray manokana
            if (menageId.HasValue)
            {
                query = query.Where(c => c.MenageId == menageId.Value);
            }

            return await query.ToListAsync();
        }

        // 2. GET: api/citoyens/{id} (Maka olona iray amin'ny alalan'ny Id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Citoyen>> GetCitoyen(int id)
        {
            var citoyen = await _context.Citoyens.FindAsync(id);

            if (citoyen == null)
            {
                return NotFound(new { message = "Tsy hita io olona io ao amin'ny lisitra." });
            }

            return citoyen;
        }

        // 3. POST: api/citoyens (Fampidirana mponina vaovao)
        [HttpPost]
        public async Task<IActionResult> CreateCitoyen(CreateCitoyenDto request)
        {
            // 1. Jerena aloha raha misy ilay MenageId homena azy
            var menageExiste = await _context.Menages.AnyAsync(m => m.Id == request.MenageId);
            if (!menageExiste)
            {
                return BadRequest(new { message = "Tsy misy io tokantrano (Ménage) io ao amin'ny rafitra." });
            }

            // 2. Raha faritana ho 'Chef de ménage' izy, jerena raha efa misy Chef hafa ao amin'io tokantrano io
            if (request.EstChefDeMenage)
            {
                var mananaChefEfa = await _context.Citoyens.AnyAsync(c => c.MenageId == request.MenageId && c.EstChefDeMenage);
                if (mananaChefEfa)
                {
                    return BadRequest(new { message = "Efa manana Chef de ménage io tokantrano io. Tsy afaka asiana roa." });
                }
            }

            // 3. Famoronana ny Objet Citoyen
            var citoyen = new Citoyen
            {
                Nom = request.Nom,
                Prenoms = request.Prenoms,
                Sexe = request.Sexe,
                DateNaissance = request.DateNaissance,
                PhotoUrl = request.PhotoUrl,
                MenageId = request.MenageId,
                EstChefDeMenage = request.EstChefDeMenage
            };

            _context.Citoyens.Add(citoyen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCitoyen), new { id = citoyen.Id }, citoyen);
        }

        // 4. PUT: api/citoyens/{id} (Fanovana ny mombamomba ny olona iray)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCitoyen(int id, CreateCitoyenDto request)
        {
            var citoyen = await _context.Citoyens.FindAsync(id);
            if (citoyen == null)
            {
                return NotFound(new { message = "Tsy hita io olona io mba hovana." });
            }

            // Jerena indray ny momba ny Chef de ménage raha novonany ho true izany
            if (request.EstChefDeMenage && !citoyen.EstChefDeMenage)
            {
                var mananaChefEfa = await _context.Citoyens.AnyAsync(c => c.MenageId == request.MenageId && c.EstChefDeMenage && c.Id != id);
                if (mananaChefEfa)
                {
                    return BadRequest(new { message = "Efa manana Chef de ménage io tokantrano io." });
                }
            }

            // Fanavaozana ny angona
            citoyen.Nom = request.Nom;
            citoyen.Prenoms = request.Prenoms;
            citoyen.Sexe = request.Sexe;
            citoyen.DateNaissance = request.DateNaissance;
            citoyen.PhotoUrl = request.PhotoUrl;
            citoyen.MenageId = request.MenageId;
            citoyen.EstChefDeMenage = request.EstChefDeMenage;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Citoyens.AnyAsync(c => c.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Voaova soa aman-tsara ny mombamomba ilay olona!", data = citoyen });
        }

        // 5. DELETE: api/citoyens/{id} (Famafana olona iray)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitoyen(int id)
        {
            var citoyen = await _context.Citoyens.FindAsync(id);
            if (citoyen == null)
            {
                return NotFound(new { message = "Tsy hita io olona io." });
            }

            _context.Citoyens.Remove(citoyen);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Voafafa soa aman-tsara ilay olona ao amin'ny lisitra." });
        }
    }
}