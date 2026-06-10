using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Application.DTOs;

public class UpdateProductoDto
{
    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Stock { get; set; }
}
