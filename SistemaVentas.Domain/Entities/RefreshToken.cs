namespace SistemaVentas.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }

    public bool IsRevoked { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
