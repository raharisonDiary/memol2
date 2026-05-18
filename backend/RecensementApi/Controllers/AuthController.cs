using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecensementApi.Data;
using RecensementApi.Models;
using RecensementApi.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace RecensementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; 

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // 1. INSCRIPTION : POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            // Jerena aloha raha efa misy ny email
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Efa misy mampiasa io Email io.");
            }

            // Hash-na ny password amin'ny alalan'ny BCrypt na SHA256 tsotra (eto ampiasaintsika SHA256)
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var passwordHash = Convert.ToBase64String(hashedBytes);

            var utilisateur = new Utilisateur
            {
                Nom = request.Nom,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
                RegionAffectation = request.RegionAffectation,
                DateCreation = DateTime.UtcNow
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Voasoratra soa aman-tsara ny mpampiasa vaovao!" });
        }

        // 2. CONNEXION : POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var utilisateur = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (utilisateur == null)
            {
                return BadRequest("Tsy misy io mpampiasa io na diso ny email.");
            }

            // Jerena raha mifanaraka ny password hash
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var inputHash = Convert.ToBase64String(hashedBytes);

            if (utilisateur.PasswordHash != inputHash)
            {
                return BadRequest("Diso ny teny miafina (Password).");
            }

            // Eto aloha dia averintsika tsotra ny mombamomba azy. 
            // Rehefa mametraka JWT Token isika dia hovana eto.
            return Ok(new
            {
                message = "Tafiditra ianao!",
                id = utilisateur.Id,
                nom = utilisateur.Nom,
                email = utilisateur.Email,
                role = utilisateur.Role,
                region = utilisateur.RegionAffectation
            });
        }
    }
}