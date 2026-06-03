using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Services;

// Implementar la clase ProductoService que implementa la interfaz IProductoService 
public class ProductoService : IProductoService
{
    // Inyectar el repositorio de productos a través del constructor para acceder a los métodos de acceso a datos
    private readonly IProductoRepository _repository;

    // El constructor recibe una instancia de IProductoRepository y la asigna a un campo privado para su uso en los métodos del servicio
    public ProductoService(IProductoRepository repository)
    {
        // Asignar la instancia del repositorio al campo privado para su uso en los métodos del servicio
        _repository = repository;
    }

    // Implementar el método GetAllAsync que obtiene todos los productos utilizando el repositorio
    public async Task<IEnumerable<ProductoResponseDto>> GetAllAsync()
    {
        var productos = await _repository.GetAllAsync();

        return productos.Select(p =>
            new ProductoResponseDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Stock = p.Stock
            }
        );
    }

    // Implementar el método GetByIdAsync que obtiene un producto por su ID utilizando el repositorio
    public async Task<ProductoResponseDto?> GetByIdAsync(int id)
    {
        var producto = await _repository.GetByIdAsync(id);

        if (producto == null)
        {
            throw new Exception("Producto no encontrado");
        }

        return new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    // Implementar el método CreateAsync que crea un nuevo producto utilizando el repositorio
    public async Task<ProductoResponseDto> CreateAsync(CreateProductoDto dto)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            Stock = dto.Stock
        };

        await _repository.AddAsync(producto);

        return new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    // Implementar el método UpdateAsync que actualiza un producto existente utilizando el repositorio
    public async Task<ProductoResponseDto?> UpdateAsync(int id, UpdateProductoDto dto)
    {
        var producto = await _repository.GetByIdAsync(id);

        if (producto == null)
        {
            return null;
        }

        producto.Nombre = dto.Nombre;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        await _repository.UpdateAsync(producto);

        return new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    // Implementar el método DeleteAsync que elimina un producto por su ID utilizando el repositorio
    public async Task<bool> DeleteAsync(int id)
    {
        var producto = await _repository.GetByIdAsync(id);

        if (producto == null)
        {
            return false;
        }
        
        await _repository.DeleteAsync(id);
        
        return true;
    }
}
