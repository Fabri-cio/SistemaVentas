namespace SistemaVentas.Application.DTOs;

public class ErrorResponseDto
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}
