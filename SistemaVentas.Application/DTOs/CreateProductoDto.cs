using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Application.DTOs;

// DTO pare crear productos desde la API
public class CreateProductoDto
{
    // Data Annotations para validar los datos de entrada
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    // El precio debe ser mayor que cero
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio deber ser mayor que cero")]
    public decimal Precio { get; set; }

    // El stock no puede ser negativo
    [Range(0,int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
}
