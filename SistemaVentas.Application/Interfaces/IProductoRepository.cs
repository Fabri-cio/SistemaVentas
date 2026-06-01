using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Interfaces;

// Define la interfaz para el repositorio de productos el contrato que debe cumplir cualquier implementación de repositorio de productos
public interface IProductoRepository
{
    // Agrega un método para obtener todos los productos
    Task<IEnumerable<Producto>> GetAllAsync();

    // Agrega un método para obtener un producto por su ID
    Task<Producto?> GetByIdAsync(int id);

    // Agrega un método para agregar un nuevo producto
    Task AddAsync(Producto producto);

    // Agrega un método para actualizar un producto existente
    Task UpdateAsync(Producto producto);

    // Agrega un método para eliminar un producto por su ID
    Task DeleteAsync(int id);

}
