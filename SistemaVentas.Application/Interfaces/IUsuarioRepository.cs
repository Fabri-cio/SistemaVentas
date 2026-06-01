using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);

        Task AddAsync(Usuario usuario);
    }
}
