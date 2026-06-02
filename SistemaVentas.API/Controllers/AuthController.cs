using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;

namespace SistemaVentas.API.Controllers;

// Controlador para gestionar la autenticacion de usuarios
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // Endpoint POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUsuarioDto dto)
    {
        await _service.RegisterAsync(dto);

        return Ok("Usuario registrado exitosamente");
    }

    // Endpoint POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var resultado = await _service.LoginAsync(dto);

        return Ok(resultado);
    }
}
