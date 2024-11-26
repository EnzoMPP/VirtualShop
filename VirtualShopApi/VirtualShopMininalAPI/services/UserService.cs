// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace VirtualShopMinimalAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly string _jwtKey;

        public UserService(AppDbContext db, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _jwtKey = configuration["Jwt:Key"];
        }

        public async Task<IResult> RegisterUser(User user)
        {
            if (_db.Users == null)
            {
                return Results.NotFound("Users collection is null.");
            }

            if (string.IsNullOrEmpty(user.NomeUsuario) || string.IsNullOrEmpty(user.Email))
            {
                return Results.BadRequest("NomeUsuario e Email são obrigatórios.");
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Results.Conflict("Email já está em uso.");
            }

            if (!string.IsNullOrEmpty(user.Cpf))
            {
                var existingCpf = await _db.Users.FirstOrDefaultAsync(u => u.Cpf == user.Cpf);
                if (existingCpf != null)
                {
                    return Results.Conflict("CPF já está em uso.");
                }
            }

            if (!string.IsNullOrEmpty(user.Senha))
            {
                user.Senha = _passwordHasher.HashPassword(user, user.Senha);
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Results.Ok(user);
        }

        public async Task<IResult> LoginUser(LoginRequest loginRequest)
        {
            if (_db.Users == null)
            {
                return Results.NotFound("Users collection is null.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null || user.Senha == null || string.IsNullOrEmpty(loginRequest.Senha))
            {
                return Results.Unauthorized();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Senha, loginRequest.Senha);
            if (result == PasswordVerificationResult.Failed)
            {
                return Results.Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString())
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

            return Results.Ok(new { Token = tokenString });
        }

        public async Task<IResult> GetUserProfile(string email)
        {
            if (_db.Users == null)
            {
                return Results.NotFound("Users collection is null.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Results.NotFound("Usuário não encontrado.");
            }

             return Results.Ok(new { user.Id, user.NomeUsuario, user.Email, user.Cpf, user.IsAdmin });
        }

        public async Task<IResult> UpdateUserProfile(string email, User updatedUser)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Results.NotFound("Usuário não encontrado.");
            }

            user.NomeUsuario = updatedUser.NomeUsuario ?? user.NomeUsuario;
            user.Email = updatedUser.Email ?? user.Email;

            if (!string.IsNullOrEmpty(updatedUser.Senha))
            {
                user.Senha = _passwordHasher.HashPassword(user, updatedUser.Senha);
            }

            await _db.SaveChangesAsync();
            return Results.Ok(new { Mensagem = "Perfil atualizado com sucesso" });
        }

        public async Task<IResult> AddAdmin(User admin)
        {
            if (_db.Users == null)
            {
                return Results.NotFound("Users collection is null.");
            }

            admin.IsAdmin = true;

            if (!string.IsNullOrEmpty(admin.Senha))
            {
                admin.Senha = _passwordHasher.HashPassword(admin, admin.Senha);
            }

            _db.Users.Add(admin);
            await _db.SaveChangesAsync();
            return Results.Ok(admin);
        }

        // Novo método para obter produtos comprados pelo usuário
        public async Task<IResult> GetPurchasedProducts(string email)
        {
            var user = await _db.Users
                .Include(u => u.Sales)
                    .ThenInclude(s => s.SaleProducts)
                        .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Results.NotFound("Usuário não encontrado.");
            }

            var produtosComprados = user.Sales
                .SelectMany(s => s.SaleProducts)
                .Select(sp => sp.Product)
                .Where(p => p != null)
                .Distinct()
                .ToList();

            return Results.Ok(produtosComprados);
        }
    }
}