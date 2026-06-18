namespace SistemaVentas.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}
