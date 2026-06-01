using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Domain.Entities;
using SistemaVentas.Infrastructure.Data;

namespace SistemaVentas.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // Implementación de los métodos de IUsuarioRepository
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    // Implementación del método para agregar un nuevo usuario
    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }
}
