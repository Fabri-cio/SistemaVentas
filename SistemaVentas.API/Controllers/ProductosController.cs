using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    }
}
