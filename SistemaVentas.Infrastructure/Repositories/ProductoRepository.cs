using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;
using SistemaVentas.Infrastructure.Data;

namespace SistemaVentas.Infrastructure.Repositories;

// Implementa la interfaz IProductoRepository utilizando Entity Framework Core para interactuar con la base de datos
public class ProductoRepository : IProductoRepository
{
    // Inyecta el contexto de la base de datos para acceder a los productos
    private readonly AppDbContext _context;

    // Constructor que recibe el contexto de la base de datos a través de la inyección de dependencias
    public ProductoRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implementa el método para obtener todos los productos de la base de datos
    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _context.Productos.ToListAsync();
    }

    // Implementa el método para obtener productos paginados de la base de datos
    public async Task<PagedResponse<Producto>> GetPagedAsync(ProductoQueryDto query)
    {
        // Crea una consulta base para los productos utilizando AsQueryable para permitir la construcción dinámica de la consulta
        var productosQuery = _context.Productos.AsQueryable();

        // Filtro por Nombre
        if (!string.IsNullOrWhiteSpace(query.Nombre))
        {
            // Filtra los productos cuyo nombre contiene el valor proporcionado en la consulta
            productosQuery = productosQuery.Where(p => p.Nombre.Contains(query.Nombre));
        }

        // Filtro por Precio Minimo
        if (query.PrecioMin.HasValue)
        {
            // Filtra los productos cuyo precio es mayor o igual al valor mínimo proporcionado en la consulta
            productosQuery = productosQuery.Where(p => p.Precio >= query.PrecioMin.Value);
        }

        // Filtro por Precio Maximo
        if (query.PrecioMax.HasValue)
        {
            // Filtra los productos cuyo precio es menor o igual al valor máximo proporcionado en la consulta
            productosQuery = productosQuery.Where(p => p.Precio <= query.PrecioMax.Value);
        }

        // Calcula el total de registros para la paginación
        var totalRecords = await productosQuery.CountAsync();

        // Obtiene los productos paginados utilizando Skip y Take
        var productos = await productosQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Devuelve la respuesta paginada con los productos obtenidos
        return new PagedResponse<Producto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalRecords = totalRecords,
            Data = productos
        };
    }

    // Implementa el método para obtener un producto por su ID de la base de datos
    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await _context.Productos.FindAsync(id);
    }

    // Implementa el método para agregar un nuevo producto a la base de datos
    public async Task AddAsync(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
    }

    // Implementa el método para actualizar un producto existente en la base de datos
    public async Task UpdateAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    // Implementa el método para eliminar un producto por su ID de la base de datos
    public async Task DeleteAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
