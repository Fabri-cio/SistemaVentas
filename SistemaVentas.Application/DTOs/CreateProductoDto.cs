namespace SistemaVentas.Application.DTOs;

// DTO pare crear productos desde la API
public class CreateProductoDto
{
    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Stock { get; set; }
}
