// GET /auth/google-login
// GET /auth/signin-google

// UserController
// POST /api/user/register
// POST /api/user/login
// POST /api/user/logout
// POST /api/user/add-admin
// GET /api/user/profile

// ProductController
// GET /api/product
// GET /api/product/{id}
// POST /api/product
// GET /api/product/search

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LojaVirtualAPI.Data;
using LojaVirtualAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=lojaVirtual.db"));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var configuracaoJwt = builder.Configuration.GetSection("Jwt");
var chaveJwt = configuracaoJwt["Key"];
var expiresInMinutesConfig = configuracaoJwt["ExpiresInMinutes"];

if (string.IsNullOrEmpty(chaveJwt))
{
    throw new InvalidOperationException("A chave JWT não está configurada corretamente.");
}

if (!double.TryParse(expiresInMinutesConfig, out double expiresInMinutes))
{
    throw new InvalidOperationException("A configuração 'Jwt:ExpiresInMinutes' é inválida ou não está definida.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt))
    };
})
.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection =
        builder.Configuration.GetSection("Authentication:Google");

    options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured.");
    options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is not configured.");
    options.CallbackPath = "/signin-google";
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    contexto.Database.Migrate();

    if (contexto.Users != null && !contexto.Users.Any(u => u.Email == "admin@admin.com"))
    {
        var usuarioAdmin = new User
        {
            Name = "Admin",
            CPF = "00000000000",
            Email = "admin@admin.com",
            Password = BCrypt.Net.BCrypt.HashPassword("admin"),
            IsAdmin = true
        };
        contexto.Users.Add(usuarioAdmin);
        contexto.SaveChanges();
    }
}

app.Run();