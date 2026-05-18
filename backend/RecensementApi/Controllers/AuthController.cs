using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecensementApi.Data;
using RecensementApi.DTOs;
using RecensementApi.Models;

namespace RecensementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<Utilisateur>> Register(RegisterDto dto)
        {
            // Fanamarihana mampiasa u.Email
            if (await _context.Utilisateurs.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Efa misy mampiasa io Email io.");
            }

            // Hashing ny password mampiasa BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var mpampiasa = new Utilisateur
            {
                Nom = dto.Nom,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role,
                RegionAffectation = dto.RegionAffectation
            };

            _context.Utilisateurs.Add(mpampiasa);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Voasoratra anarana soa aman-tsara!" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Fitadiavana ny mpampiasa amin'ny alalan'ny Email
            var mpampiasa = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (mpampiasa == null || !BCrypt.Net.BCrypt.Verify(dto.Password, mpampiasa.PasswordHash))
            {
                return Unauthorized("Diso ny Email na ny Password.");
            }

            // Famoronana Token JWT
            var token = GenerateJwtToken(mpampiasa);

            return Ok(new
            {
                Token = token,
                Nom = mpampiasa.Nom,
                Role = mpampiasa.Role,
                RegionAffectation = mpampiasa.RegionAffectation
            });
        }

        private string GenerateJwtToken(Utilisateur user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tehirizina ao anatin'ny Token ny mombamomba azy
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("RegionAffectation", user.RegionAffectation ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Manankery 1 andro
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}