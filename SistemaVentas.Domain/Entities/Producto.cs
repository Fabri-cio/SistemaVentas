namespace SistemaVentas.Domain.Entities

// Representa la tabla productos en la base de datos
public class Producto
{
    // Clave primaria
    public int Id { get; set; }
    // Nombre del producto
    public string Nombre { get; set; } = string.Empty;
    // Precio del producto
    public decimal Precio { get; set; }
    // Cantidad disponible en stock
    public int Stock { get; set; }

}