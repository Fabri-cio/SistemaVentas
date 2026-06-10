using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;

namespace SistemaVentas.API.Controllers;

// Indica que es un controlador API
[ApiController]
// Ruta base: /api/productos
[Route("api/productos")]
[Authorize] // Proteger todo el controlador
public class ProductosController : BaseController
{
    // Servicio para gestionar la logica de negocio relacionada con productos
    private readonly IProductoService _service;

    // Inyeccion de dependencias del servicio de productos
    public ProductosController(IProductoService service)
    {
        _service = service;
    }

    // Endpoint GET: /api/productos
    [HttpGet]
    public async Task<IActionResult> GetProductos([FromQuery] ProductoQueryDto query)
    {
        // Obtiene la lista de productos desde la base de datos de forma asincrona
        var productos = await _service.GetPagedAsync(query);

        // Retorna la lista de productos con un estado HTTP 200 OK
        return SuccessResponse(productos, "Productos obtenidos correctamente");
    }

    // Enpoint POST:
    // Post /api/productos
    [HttpPost]
    public async Task<IActionResult> CrearProducto(CreateProductoDto dto)
    {
        // Crea un nuevo producto en la base de datos utilizando el servicio de productos de forma asincrona
        var producto = await _service.CreateAsync(dto);

        // Retorna HTTP 200 con producto creado
        return CreatedResponse(producto, "Producto creado correctamente");
    }

    // Endpoint GET: /api/productos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductoById(int id)
    {
        // Busca el producto por su ID de forma asincrona
        var producto = await _service.GetByIdAsync(id);


        return SuccessResponse(
            producto,
            "Producto obtenido correctamente"
        );
    }

    // Endpoint PUT: /api/productos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarProducto(int id, UpdateProductoDto dto)
    {
        // Actualiza el producto en la base de datos utilizando el servicio de productos de forma asincrona
        var producto = await _service.UpdateAsync(id, dto);

        // Verificar si existe
        if (producto == null)
        {
            return NotFound();
        }

        return SuccessResponse(producto, "Producto actualizado correctamente");
    }

    // Endpoint DELETE: /api/productos/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Solo los usuarios con rol "Admin" pueden eliminar productos
    public async Task<IActionResult> EliminarProducto(int id)
    {
        // Busca el producto por su ID de forma asincrona
        var producto = await _service.DeleteAsync(id);

        // Si no se encuentra el producto, retorna HTTP 404 Not Found
        if (!producto)
        {
            return NotFound();
        }

        // Retorna HTTP 200 OK con mensaje de éxito
        return SuccessResponse(true, "Producto eliminado correctamente");
    }
}
