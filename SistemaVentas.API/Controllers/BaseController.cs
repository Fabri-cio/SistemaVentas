using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Application.DTOs.Responses;

namespace SistemaVentas.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(
        T data,
        string message = "Operación realizada correctamente")
    {
        return Ok(
            new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            }
        );
    }

    protected IActionResult CreatedResponse<T>(T data, string message = "Registro creado correctamente")
    {
        return Created(
            string.Empty,
            new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            }
        );
    }
}