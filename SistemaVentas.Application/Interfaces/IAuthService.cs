using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto dto);

    Task RegisterAsync(CreateUsuarioDto dto);
}
