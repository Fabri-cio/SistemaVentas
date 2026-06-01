using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Application.DTOs;

public class UpdateProductoDto
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0.01, 999999)]
    public decimal Precio { get; set; }

    [Range(0, 1000)]
    public int Stock { get; set; }
}
