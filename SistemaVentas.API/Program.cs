using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Services;
using SistemaVentas.Infrastructure.Data;
using SistemaVentas.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Configuracion Authentication con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SistemaVentas",
            ValidAudience = "SistemaVentas",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "MiClaveSuperSecretaParaSistemaVentas2026"
                 )
            )
        };
    });

// Configuracion de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuracion de DataContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Inyección de dependencias para el repositorio (Dependency Injection)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Inyección de dependencias para el repositorio de usuarios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     //genera JSON con la documentación de la API
    app.UseSwaggerUI();   //interfaz visual
}

// Redirecciona HTTP a HTTPS
app.UseHttpsRedirection();

// Habilita autenticación y autorización
app.UseAuthentication();

// Habilita autorización
app.UseAuthorization();

// Habilita Controllers
app.MapControllers();

// Ejecuta la aplicación
app.Run();