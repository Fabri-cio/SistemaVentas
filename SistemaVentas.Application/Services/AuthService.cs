using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaVentas.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUsuarioRepository repository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _repository = repository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RegisterAsync(CreateUsuarioDto dto)
    {
        var existeUsuario = await _repository.GetByEmailAsync(dto.Email);

        if (existeUsuario != null)
        {
            _logger.LogWarning(
                "Intento de registro fallido para {Email}",
                dto.Email
            );

            throw new Exception("El email ya esta registrado");
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,

            // Hash de la contraseña usando BCrypt
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

            Role = "User" // Rol por defecto
        };

        await _repository.AddAsync(usuario);

        _logger.LogInformation("Usuario registrado correctamente: {Email}", usuario.Email);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.GetByEmailAsync(dto.Email);

        if (usuario == null ||
            !BCrypt.Net.BCrypt.Verify(
                dto.Password,
                usuario.Password
            ))
        {
            _logger.LogWarning(
                "Intento de login fallido para {Email}",
                dto.Email
            );

            throw new Exception("Credenciales inválidas");
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.Name,
                usuario.Nombre
            ),

            new Claim(
                ClaimTypes.Email,
                usuario.Email
            ),

            new Claim(
                ClaimTypes.Role,
                usuario.Role
            )
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(double.Parse(_configuration["Jwt:ExpirationHours"]!)),
            signingCredentials: credentials
        );

        _logger.LogInformation(
            "Login exitoso para {Email}",
            usuario.Email
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}