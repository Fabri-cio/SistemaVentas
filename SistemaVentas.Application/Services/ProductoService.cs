using Microsoft.Extensions.Logging;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Services;

// Implementar la clase ProductoService que implementa la interfaz IProductoService 
public class ProductoService : IProductoService
{
    // Inyectar el repositorio de productos a través del constructor para acceder a los métodos de acceso a datos
    private readonly IProductoRepository _repository;
    private readonly ILogger<ProductoService> _logger;

    // El constructor recibe una instancia de IProductoRepository y la asigna a un campo privado para su uso en los métodos del servicio
    public ProductoService(IProductoRepository repository, ILogger<ProductoService> logger)
    {
        // Asignar la instancia del repositorio al campo privado para su uso en los métodos del servicio
        _repository = repository;
        _logger = logger;
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

    public async Task<PagedResponse<ProductoResponseDto>> GetPagedAsync(ProductoQueryDto query)
    {

        var pagedProductos = await _repository.GetPagedAsync(query);

        return new PagedResponse<ProductoResponseDto>
        {
            Page = pagedProductos.Page,
            PageSize = pagedProductos.PageSize,
            TotalRecords = pagedProductos.TotalRecords,

            Data = pagedProductos.Data.Select(p =>
                new ProductoResponseDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock
                }
            )
        };
    }

    // Implementar el método GetByIdAsync que obtiene un producto por su ID utilizando el repositorio
    public async Task<ProductoResponseDto?> GetByIdAsync(int id)
    {
        var producto = await _repository.GetByIdAsync(id);

        if (producto == null)
        {
            _logger.LogWarning("Producto con ID {Id} no encontrado", id);

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

        _logger.LogInformation("Producto creado correctamente. Nombre: {Nombre}, Precio: {Precio}",
            producto.Nombre,
            producto.Precio
        );

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
            _logger.LogWarning("Intento de actualización fallida para producto con ID {Id}", id);

            return null;
        }

        producto.Nombre = dto.Nombre;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;

        await _repository.UpdateAsync(producto);

        _logger.LogInformation("Producto actualizado correctamente: {Nombre}", producto.Nombre);

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
            _logger.LogWarning("Intento de eliminación fallida para producto con ID {Id}", id);

            return false;
        }

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Producto eliminado correctamente. Id: {id}, Nombre: {Nombre}",
            producto.Id,
            producto.Nombre);

        return true;
    }
}
