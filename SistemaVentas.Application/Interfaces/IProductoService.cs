using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Application.Interfaces;

// Definir la interfaz para el servicio de productos
public interface IProductoService
{
    // Definir los métodos que el servicio de productos debe implementar
    Task<IEnumerable<ProductoResponseDto>> GetAllAsync();

    // Agregar un método para obtener productos paginados
    Task<PagedResponse<ProductoResponseDto>> GetPagedAsync(ProductoQueryDto query);

    // Agregar un método para obtener un producto por su ID
    Task<ProductoResponseDto?> GetByIdAsync(int id);

    // Agregar un método para crear un nuevo producto
    Task<ProductoResponseDto> CreateAsync(CreateProductoDto dto);

    // Agregar un método para actualizar un producto existente
    Task<ProductoResponseDto?> UpdateAsync(int id, UpdateProductoDto dto);

    // Agregar un método para eliminar un producto por su ID
    Task<bool> DeleteAsync(int id);
}
