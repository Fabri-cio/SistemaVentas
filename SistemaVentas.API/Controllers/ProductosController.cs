using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Domain.Entities;
using SistemaVentas.Infrastructure.Data;

namespace SistemaVentas.API.Controllers
{
    // Indica que es un controlador API
    [ApiController]

    // Ruta base: /api/productos
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        // Conexion con la bae de datos
        private readonly AppDbContext _context;

        // Inyeccion de dependencias del contexto de datos (DbContext)
        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint GET: /api/productos
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            // Obtiene la lista de productos desde la base de datos de forma asincrona
            var productos = await _context.Productos.ToListAsync();

            // Retorna la lista de productos con un estado HTTP 200 OK
            return Ok(productos);
        }

        // Enpoint POST:
        // Post /api/productos
        [HttpPost]
        public async Task<IActionResult> CrearProducto(CreateProductoDto dto)
        {
            // Convertimos DTO a entidad
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            // Guarda producto de manera temporal en EF Core, pero no se ha guardado en la base de datos
            await _context.Productos.AddAsync(producto);

            // Guarda cambios en SQL Server
            await _context.SaveChangesAsync();

            // Retorna HTTP 200 con producto creado
            return Ok(producto);
        }
    }
}
