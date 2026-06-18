using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(CreateUsuarioDto dto);

    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

    Task LogoutAsync(string refreshtoken);
}
