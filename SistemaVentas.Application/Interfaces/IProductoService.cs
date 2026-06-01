using SistemaVentas.Application.DTOs;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Interfaces;

// Definir la interfaz para el servicio de productos
public interface IProductoService
{
    // Definir los métodos que el servicio de productos debe implementar
    Task<IEnumerable<Producto>> GetAllAsync();

    // Agregar un método para obtener un producto por su ID
    Task<Producto?> GetByIdAsync(int id);

    // Agregar un método para crear un nuevo producto
    Task<Producto> CreateAsync(CreateProductoDto dto);

    // Agregar un método para actualizar un producto existente
    Task<Producto?> UpdateAsync(int id, UpdateProductoDto dto);

    // Agregar un método para eliminar un producto por su ID
    Task<bool> DeleteAsync(int id);
}
