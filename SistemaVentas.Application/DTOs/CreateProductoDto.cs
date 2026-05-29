using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Application.DTOs
{
    // DTO pare crear productos desde la API
    public class CreateProductoDto
    {
        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01, 999999)]
        public decimal Precio { get; set; }

        [Range(0,100000)]
        public int Stock { get; set; }
    }
}
