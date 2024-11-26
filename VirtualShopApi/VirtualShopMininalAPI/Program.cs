using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using VirtualShopMinimalAPI.Data;
using Microsoft.AspNetCore.Identity;
using VirtualShopMinimalAPI.Models;
using VirtualShopMinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("defaultconnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT key is not configured or is too short.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;       
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) 
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowAll");


app.MapPost("/api/User/register", async (User user, IUserService userService) =>
{
    return await userService.RegisterUser(user);
})
.WithName("RegistrarUsuario")
.WithTags("Usuários");

app.MapPost("/api/User/login", async (LoginRequest loginRequest, IUserService userService) =>
{
    return await userService.LoginUser(loginRequest);
})
.WithName("LoginUsuario")
.WithTags("Usuários");

app.MapGet("/api/User/profile", async (HttpContext http, IUserService userService) =>
{
    var email = http.User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(email))
    {
        return Results.Unauthorized();
    }

    return await userService.GetUserProfile(email);
})
.WithName("PerfilUsuario")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPut("/api/User/profile", async (User updatedUser, HttpContext http, IUserService userService) =>
{
    var email = http.User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(email))
    {
        return Results.Unauthorized();
    }

    return await userService.UpdateUserProfile(email, updatedUser);
})
.WithName("AlterarPerfil")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPost("/api/Admin", async (User admin, IUserService userService) =>
{
    return await userService.AddAdmin(admin);
})
.WithName("AdicionarAdmin")
.WithTags("Usuários")
.RequireAuthorization();

app.MapPost("/api/Product", async (Product product, IProductService productService) =>
{
    return await productService.AddProduct(product);
})
.WithName("AdicionarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapGet("/api/Product", async (IProductService productService) =>
{
    return await productService.GetProducts();
})
.WithName("ListarProdutos")
.WithTags("Produtos");

app.MapGet("/api/Product/search", async (string? name, IProductService productService) =>
{
    return await productService.SearchProducts(name);
})
.WithName("PesquisarProdutos")
.WithTags("Produtos");

app.MapPut("/api/Product/{id}", async (int id, Product updatedProduct, IProductService productService) =>
{
    return await productService.UpdateProduct(id, updatedProduct);
})
.WithName("AlterarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapDelete("/api/Product/{id}", async (int id, IProductService productService) =>
{
    return await productService.DeleteProduct(id);
})
.WithName("DeletarProduto")
.WithTags("Produtos")
.RequireAuthorization();

app.MapPost("/api/Sale", async (SaleRequest saleRequest, ISaleService saleService) =>
{
    return await saleService.RegisterSale(saleRequest);
})
.WithName("RegistrarVenda")
.WithTags("Vendas")
.RequireAuthorization();

app.MapGet("/api/auth/google-login", async (HttpContext context) =>
{
    var props = new AuthenticationProperties { RedirectUri = "/api/auth/google-callback" };
    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
})
.WithName("GoogleLogin");

app.MapGet("/api/auth/google-callback", async (HttpContext context, AppDbContext _db) =>
{
    var result = await context.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
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
    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    var redirectUrl = $"http://localhost:3000/login?token={tokenString}";
    return Results.Redirect(redirectUrl);
})
.WithName("GoogleCallback");

app.MapGet("/api/User/{id}/purchased-products", async (int id, IUserService userService) =>
{
    return await userService.GetPurchasedProducts(id);
})
.WithName("GetPurchasedProducts")
.WithTags("Usuários")
.RequireAuthorization();


app.Run();