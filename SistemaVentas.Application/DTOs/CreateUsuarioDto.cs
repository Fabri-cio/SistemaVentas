using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Application.DTOs;

public class CreateUsuarioDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}