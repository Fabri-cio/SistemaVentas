using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;

namespace SistemaVentas.API.Controllers;

// Controlador para gestionar la autenticacion de usuarios
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService service)
    {
        _authService = service;
    }

    // Endpoint POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUsuarioDto dto)
    {
        await _authService.RegisterAsync(dto);

        return Ok("Usuario registrado exitosamente");
    }

    // Endpoint POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var resultado = await _authService.LoginAsync(dto);

        return Ok(resultado);
    }

    // Endpoint POST: /api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        return Ok(result);
    }

    // Enpoint POST: /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequestDto dto)
    {
        await _authService.LogoutAsync(dto.RefreshToken);

        return Ok("Logout realizado correctamente");
    }
}
