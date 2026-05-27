using Microsoft.EntityFrameworkCore;

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
}