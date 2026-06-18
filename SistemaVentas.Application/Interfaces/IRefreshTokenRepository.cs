using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task UpdateAsync(RefreshToken refreshToken);
}
