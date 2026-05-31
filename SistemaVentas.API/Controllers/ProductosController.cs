using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.API.Controllers
{
    // Indica que es un controlador API
    [ApiController]

    // Ruta base: /api/productos
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        // Repositorio para acceder a los datos de productos
        private readonly IProductoRepository _repository;

        // Inyeccion de dependencias del repositorio de productos
        public ProductosController(IProductoRepository repository)
        {
            _repository = repository;
        }

        // Endpoint GET: /api/productos
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            // Obtiene la lista de productos desde la base de datos de forma asincrona
            var productos = await _repository.GetAllAsync();

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

            await _repository.AddAsync(producto);

            // Retorna HTTP 200 con producto creado
            return Ok(producto);
        }

        // Endpoint GET: /api/productos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(int id)
        {
            // Busca el producto por su ID de forma asincrona
            var producto = await _repository.GetByIdAsync(id);

            // Si no se encuentra el producto, retorna HTTP 404 Not Found
            if (producto == null)
            {
                return NotFound();
            }

            // Si se encuentra el producto, retorna HTTP 200 OK con el producto
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, UpdateProductoDto dto)
        {
            // Buscar producto por ID
            var producto = await _repository.GetByIdAsync(id);

            // Verificar si existe
            if (producto == null)
            {
                return NotFound();
            }

            // Actualizar propiedades del producto con los datos del DTO
            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;

            // Guardar cambios en la base de datos
            await _repository.UpdateAsync(producto);

            return Ok(producto);
        }

        // Endpoint DELETE: /api/productos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            // Busca el producto por su ID de forma asincrona
            var producto = await _repository.GetByIdAsync(id);

            // Si no se encuentra el producto, retorna HTTP 404 Not Found
            if (producto == null)
            {
                return NotFound();
            }

            // Elimina el producto de la base de datos
            await _repository.DeleteAsync(id);

            // Retorna HTTP 200 OK con mensaje de éxito
            return Ok("Producto eliminado correctamente");
        }
    }
}
