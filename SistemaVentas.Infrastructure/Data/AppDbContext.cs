using Microsoft.EntityFrameworkCore;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Infrastructure.Data;

// Clase principal de conexion de la base de datos, hereda de DbContext
public class AppDbContext : DbContext
{
    // Recibe configuracion de Entity Framework Core y SQL Server a traves de inyeccion de dependencias
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Representa la tabla productos en SQL Server
    public DbSet<Producto> Productos { get; set; }

    // Representa la tabla usuarios en SQL Server
    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Conigura el modelo de datos, se puede usar para establecer relaciones, restricciones, etc.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura precision del precio: 18 digitos en total, 2 decimales
        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasPrecision(18, 2); // Configura el tipo decimal con precision y escala

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Usuario)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UsuarioId);
    }


}