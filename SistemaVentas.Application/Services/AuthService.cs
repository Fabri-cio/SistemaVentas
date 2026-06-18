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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUsuarioRepository repository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _repository = repository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _logger = logger;
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
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

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
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

        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UsuarioId = usuario.Id,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            AccessToken = new JwtSecurityTokenHandler()
                .WriteToken(token),

            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        // Busca el refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        // Valida si existe el refresh token
        if (storedToken == null)
        {
            throw new Exception("Refresh Token Invalido");
        }

        // Valida que no este revocado
        if (storedToken.IsRevoked)
        {
            throw new Exception("Refresh Token revocado");
        }

        // Valida que no este expirado
        if (storedToken.ExpirationDate < DateTime.UtcNow)
        {
            throw new Exception("Refresh Token Expirado");
        }

        // Obtine el usuario dueño del token
        var usuario = storedToken.Usuario;

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
                    _configuration["Jwt:key"]!
                )
            );

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        // Genera un nuevo JWT
        var jwtToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(
                double.Parse(
                    _configuration["Jwt:ExpirationHours"]!
                )
            ),
            signingCredentials: credentials
        );

        // Revoca el refresh token anterior
        storedToken.IsRevoked = true;

        await _refreshTokenRepository.UpdateAsync(storedToken);

        // Genera un nuevo token
        var newRefreshToken = GenerateRefreshToken();

        // Lo guarda en al base de datos
        await _refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                Token = newRefreshToken,
                UsuarioId = usuario.Id,
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            }
        );

        // Retorna ambos tokens 
        return new AuthResponseDto
        {
            AccessToken =
                new JwtSecurityTokenHandler()
                    .WriteToken(jwtToken),

            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null)
        {
            throw new Exception("Refresh Token Invalido");
        }

        storedToken.IsRevoked = true;

        await _refreshTokenRepository.UpdateAsync(storedToken);

        _logger.LogInformation(
            "Logout realizado correctamente por el usuario {UsuarioId}",
            storedToken.UsuarioId
        );
    }

}
