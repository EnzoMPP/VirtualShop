using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VirtualShopMinimalAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly string _jwtKey;

        public AuthService(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _jwtKey = configuration["Jwt:Key"];
        }

        public async Task<IResult> GoogleLogin(HttpContext http)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "http://localhost:5000/api/auth/google-callback"
            };
            await http.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
            return Results.Challenge(properties);
        }

        public async Task<IResult> GoogleCallback(HttpContext http)
        {
            var result = await http.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return Results.Unauthorized();
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var nome = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nome))
            {
                return Results.BadRequest("Nome e Email são obrigatórios.");
            }

            var usuarioExistente = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (usuarioExistente == null)
            {
                var novoUsuario = new User
                {
                    NomeUsuario = nome,
                    Email = email,
                    IsAdmin = false
                };

                _db.Users.Add(novoUsuario);
                await _db.SaveChangesAsync();
                usuarioExistente = novoUsuario;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuarioExistente.Email),
                new Claim("IsAdmin", usuarioExistente.IsAdmin.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var redirectUrl = $"http://localhost:3000/home?token={tokenString}";
            return Results.Redirect(redirectUrl);
        }
    }
}
