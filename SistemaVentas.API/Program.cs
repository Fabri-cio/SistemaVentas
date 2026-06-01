using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Infrastructure.Data;
using SistemaVentas.Infrastructure.Repositories;
using SistemaVentas.Application.Services;
using Microsoft.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args);

// Agreda soporte para controlllers
builder.Services.AddControllers();

// Configuracion de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexion con SQL Server LocalDB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Inyección de dependencias para el repositorio
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Inyección de dependencias para el servicio
builder.Services.AddScoped<IProductoService, ProductoService>();

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     //genera JSON con la documentación de la API
    app.UseSwaggerUI();   //interfaz visual
}

app.UseHttpsRedirection();

// Habilita Controllers
app.MapControllers();


app.Run();