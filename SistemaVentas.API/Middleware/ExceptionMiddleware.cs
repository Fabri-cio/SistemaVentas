using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Exceptions;
using System.Text.Json;

namespace SistemaVentas.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode =
                    ex switch
                    {
                        UnauthorizedAccessException => 401, // No autorizado

                        NotFoundException => 404, // No encontrado

                        _ => 500 // Error interno del servidor
                    };

                var response = new ErrorResponseDto { Mensaje = ex.Message };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);

            }
        }
    }
}
