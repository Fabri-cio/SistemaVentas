using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Services;
using SistemaVentas.Domain.Entities;
using SistemaVentas.Application.Exceptions;
using System;
using System.Threading.Tasks;
using SistemaVentas.Application.DTOs;

namespace SistemaVentas.Tests.Services;

public class ProductoServiceTests
{
    [Fact]
    public async Task CreateAsync_DebeLlamarAddAsyncUnaVez()
    {
        // Arrange

        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        var service = new ProductoService(repositoryMock.Object, loggerMock.Object);

        var dto = new CreateProductoDto { Nombre = "Laptop", Precio = 5000, Stock = 10 };

        // Act

        await service.CreateAsync(dto);

        // Assert

        repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Producto>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_DebeCrearProductoCorretamente()
    {
        // Arrange

        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        var service = new ProductoService(repositoryMock.Object, loggerMock.Object);

        var dto = new CreateProductoDto { Nombre = "Laptop", Precio = 5000, Stock = 10 };

        // Act

        var resultado = await service.CreateAsync(dto);

        // Assert

        resultado.Should().NotBeNull();

        resultado.Nombre.Should().Be("Laptop");

        resultado.Precio.Should().Be(5000);

        resultado.Stock.Should().Be(10);
    }

    [Fact]
    public async Task GetByIdAsync_DebeRetornarProducto_CuandoExiste()
    {
        // Arrange (preparar)
        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(
                new Producto
                {
                    Id = 1,
                    Nombre = "Laptop",
                    Precio = 5000,
                    Stock = 100
                }
            );

        var service =
            new ProductoService(
                repositoryMock.Object,
                loggerMock.Object
            );

        // Act (Ejecutar)

        var resultado =
            await service.GetByIdAsync(1);

        // Assert (Verificar)

        resultado.Should().NotBeNull();

        resultado!.Id.Should().Be(1);

        resultado.Nombre.Should().Be("Laptop");

        resultado.Precio.Should().Be(5000);

        resultado.Stock.Should().Be(100);
    }

    [Fact]
    public async Task GetByIdAsync_DebeLanzarNotFoundException_CuandoNoExiste()
    {
        // Arrange (preparar)
        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Producto?)null);

        var service =
            new ProductoService(
                repositoryMock.Object,
                loggerMock.Object
            );

        // Act (Ejecutar)

        Func<Task> accion = async () => await service.GetByIdAsync(1);

        // Assert (Verificar)

        await accion.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Producto no encontrado");
    }

    [Fact]
    public async Task UpdateAsync_DebeActualizarProductoCorrectamente()
    {
        // Arrange

        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        var producto = new Producto { Id = 1, Nombre = "Laptop", Precio = 1000, Stock = 56 };

        repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(producto);

        var service = new ProductoService(repositoryMock.Object, loggerMock.Object);

        var dto = new UpdateProductoDto
        {
            Nombre = "Mouse",
            Precio = 1000,
            Stock = 20
        };

        // Act 

        var resultado = await service.UpdateAsync(1, dto);

        // Assert

        resultado.Should().NotBeNull();

        resultado!.Nombre.Should().Be("Mouse");

        resultado.Precio.Should().Be(1000);

        resultado.Stock.Should().Be(20);
    }

    [Fact]
    public async Task UpdateAsync_DebeLlamarRepositorioUnaVez()
    {
        // Arrange

        var repositoryMock = new Mock<IProductoRepository>();

        var loggerMock = new Mock<ILogger<ProductoService>>();

        var producto = new Producto
        {
            Id = 1,
            Nombre = "Parlantes",
            Precio = 3000,
            Stock = 1
        };

        repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(producto);

        var service = new ProductoService(repositoryMock.Object, loggerMock.Object);

        var dto = new UpdateProductoDto
        {
            Nombre = "USB",
            Precio = 5,
            Stock = 1
        };

        // Act

        await service.UpdateAsync(1, dto);

        // Assert

        repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Producto>()),
            Times.Once
        );
    }
}