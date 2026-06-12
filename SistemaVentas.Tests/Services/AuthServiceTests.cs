using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaVentas.Application.DTOs;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Services;
using SistemaVentas.Domain.Entities;

namespace SistemaVentas.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_DebeRegistrarUsuario()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var service = new AuthService(
            repositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new CreateUsuarioDto
        {
            Nombre = "William",
            Email = "will@test.com",
            Password = "123456"
        };

        // Act

        await service.RegisterAsync(dto);

        // Assert

        repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Usuario>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RegisterAsync_DebeLanzarExcepcion_SiEmailExiste()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new Usuario
                {
                    Id = 1,
                    Nombre = "William",
                    Email = "will@test.com"
                });

        var service = new AuthService(
            repositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new CreateUsuarioDto
        {
            Nombre = "William",
            Email = "will@test.com",
            Password = "123456"
        };

        // Act

        Func<Task> accion = async () =>
            await service.RegisterAsync(dto);

        // Assert

        await accion.Should()
            .ThrowAsync<Exception>()
            .WithMessage("El email ya esta registrado");
    }

    [Fact]
    public async Task LoginAsync_DebeRetornarToken_CuandoCredencialesSonValidas()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();

        var configurationMock = new Mock<IConfiguration>();

        configurationMock.Setup(x => x["Jwt:Key"])
            .Returns("MiClaveSuperSegura123456789012345");

        configurationMock.Setup(x => x["Jwt:Issuer"])
            .Returns("SistemaVentas");

        configurationMock.Setup(x => x["Jwt:Audience"])
            .Returns("SistemaVentasUsers");

        configurationMock.Setup(x => x["Jwt:ExpirationHours"])
            .Returns("2");

        var loggerMock = new Mock<ILogger<AuthService>>();

        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new Usuario
                {
                    Id = 1,
                    Nombre = "William",
                    Email = "will@test.com",
                    Password = passwordHash,
                    Role = "User"
                });

        var service = new AuthService(
            repositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new LoginDto
        {
            Email = "will@test.com",
            Password = "123456"
        };

        // Act

        var token = await service.LoginAsync(dto);

        // Assert

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginAsync_DebeLanzarExcepcion_CuandoUsuarioNoExiste()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var service = new AuthService(
            repositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new LoginDto
        {
            Email = "noexiste@test.com",
            Password = "123456"
        };

        // Act

        Func<Task> accion = async () =>
            await service.LoginAsync(dto);

        // Assert

        await accion.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Credenciales inválidas");
    }

    [Fact]
    public async Task LoginAsync_DebeLanzarExcepcion_CuandoPasswordEsIncorrecta()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword("123456");

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new Usuario
                {
                    Id = 1,
                    Nombre = "William",
                    Email = "will@test.com",
                    Password = passwordHash,
                    Role = "User"
                });

        var service = new AuthService(
            repositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new LoginDto
        {
            Email = "will@test.com",
            Password = "999999"
        };

        // Act

        Func<Task> accion = async () =>
            await service.LoginAsync(dto);

        // Assert

        await accion.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Credenciales inválidas");
    }
}
