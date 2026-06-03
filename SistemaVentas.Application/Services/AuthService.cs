using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;
using System.Security.Claims;
using System.Text;

namespace SistemaVentas.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;

    public AuthService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task RegisterAsync(CreateUsuarioDto dto)
    {
        var existeUsuario = await _repository.GetByEmailAsync(dto.Email);

        if (existeUsuario != null)
        {
            throw new Exception("El email ya esta registrado");
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,

            // Hash de la contraseña usando BCrypt
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        await _repository.AddAsync(usuario);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.GetByEmailAsync(dto.Email);

        if (usuario == null)
        {
            throw new Exception("Usuario no encontrado");
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password))
        {
            throw new Exception("Password incorrecto");
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
            )
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "MiClaveSuperSecretaParaSistemaVentas2026"
                )
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            issuer: "SistemaVentas",
            audience: "SistemaVentas",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}