using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Services;
using SistemaVentas.Infrastructure.Data;
using SistemaVentas.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                 )
            )
        };
    });

// Configuracion de Swagger
builder.Services.AddEndpointsApiExplorer();

// Configuracion de Swagger con seguridad JWT
builder.Services.AddSwaggerGen(options =>
{
    // Configura la información básica de la API en Swagger
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "SistemaVentas API",
            Version = "v1"
        });

    // Configura la seguridad JWT en Swagger para que los usuarios puedan autenticarse y probar los endpoints protegidos
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingrese el token JWT",
        });

    // Agrega el requisito de seguridad para que Swagger sepa que los endpoints protegidos requieren autenticación JWT
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<String>()
            }
        });
});

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