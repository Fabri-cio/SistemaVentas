using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;

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
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Password = dto.Password,
        };

        await _repository.AddAsync(usuario);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var usuario = await _repository.GetByEmailAsync(dto.Email);

        if (usuario == null || usuario.Password != dto.Password)
        {
            throw new Exception("Credenciales inválidas");
        }

        return "LOGIN_EXITOSO";
    }
}