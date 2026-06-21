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
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var service = new AuthService(
            repositoryMock.Object,
            refreshTokenRepositoryMock.Object,
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
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
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
            refreshTokenRepositoryMock.Object,
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

        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

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
            refreshTokenRepositoryMock.Object,
            configurationMock.Object,
            loggerMock.Object
        );

        var dto = new LoginDto
        {
            Email = "will@test.com",
            Password = "123456"
        };

        // Act

        var response = await service.LoginAsync(dto);

        // Assert

        response.Should().NotBeNull();

        response.AccessToken.Should()
            .NotBeNullOrWhiteSpace();

        response.RefreshToken.Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task LoginAsync_DebeLanzarExcepcion_CuandoUsuarioNoExiste()
    {
        // Arrange

        var repositoryMock = new Mock<IUsuarioRepository>();
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        repositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var service = new AuthService(
            repositoryMock.Object,
            refreshTokenRepositoryMock.Object,
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
        var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
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
            refreshTokenRepositoryMock.Object,
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

    [Fact]
    public async Task RefreshTokenAsync_DebeGenerarNuevosTokens()
    {
        // Arrange

        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "William",
            Email = "will@test.com",
            Role = "Admin"
        };

        var refreshToken = new RefreshToken
        {
            Id = 1,
            Token = "TOKEN123",
            UsuarioId = 1,
            Usuario = usuario,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        var repositoryMock =
            new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync("TOKEN123"))
            .ReturnsAsync(refreshToken);

        var configurationMock =
            new Mock<IConfiguration>();

        configurationMock.Setup(x => x["Jwt:key"])
            .Returns("MiClaveSuperSegura123456789012345");

        configurationMock.Setup(x => x["Jwt:Issuer"])
            .Returns("SistemaVentas");

        configurationMock.Setup(x => x["Jwt:Audience"])
            .Returns("SistemaVentas");

        configurationMock.Setup(x => x["Jwt:ExpirationHours"])
            .Returns("2");

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        // Act

        var result =
            await service.RefreshTokenAsync(
                "TOKEN123"
            );

        // Assert

        result.AccessToken.Should().NotBeNull();

        result.RefreshToken.Should().NotBeNull();

        refreshRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<RefreshToken>()),
            Times.Once
        );

        refreshRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<RefreshToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RefreshTokenAsync_DebeLanzarExcepcion_SiTokenNoExiste()
    {
        var repositoryMock = new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        var configurationMock =
            new Mock<IConfiguration>();

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        Func<Task> accion = async () =>
            await service.RefreshTokenAsync("TOKEN_INEXISTENTE");

        await accion.Should()
            .ThrowAsync<Exception>();
    }

    [Fact]
    public async Task RefreshTokenAsync_DebeLanzarExcepcion_SiTokenRevocado()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "William",
            Email = "will@test.com",
            Role = "Admin"
        };

        var refreshToken = new RefreshToken
        {
            Id = 1,
            Token = "TOKEN123",
            UsuarioId = 1,
            Usuario = usuario,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = true
        };

        var repositoryMock =
            new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var configurationMock =
            new Mock<IConfiguration>();

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        Func<Task> accion = async () =>
            await service.RefreshTokenAsync("TOKEN123");

        await accion.Should()
            .ThrowAsync<Exception>();
    }

    [Fact]
    public async Task RefreshTokenAsync_DebeLanzarExcepcion_SiTokenExpiro()
    {
        // Arrange

        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "William",
            Email = "will@test.com",
            Role = "Admin"
        };

        var refreshToken = new RefreshToken
        {
            Id = 1,
            Token = "TOKEN123",
            UsuarioId = 1,
            Usuario = usuario,

            // Expiró ayer
            ExpirationDate = DateTime.UtcNow.AddDays(-1),

            IsRevoked = false
        };

        var repositoryMock =
            new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var configurationMock =
            new Mock<IConfiguration>();

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        // Act

        Func<Task> accion = async () =>
            await service.RefreshTokenAsync("TOKEN123");

        // Assert

        await accion.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Refresh token expirado");
    }

    [Fact]
    public async Task LogoutAsync_DebeRevocarRefreshToken()
    {
        // Arrange

        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "William",
            Email = "will@test.com",
            Role = "Admin"
        };

        var refreshToken = new RefreshToken
        {
            Id = 1,
            Token = "TOKEN123",
            UsuarioId = 1,
            Usuario = usuario,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        var repositoryMock =
            new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync("TOKEN123"))
            .ReturnsAsync(refreshToken);

        var configurationMock =
            new Mock<IConfiguration>();

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        // Act

        await service.LogoutAsync("TOKEN123");

        // Assert

        refreshToken.IsRevoked.Should().BeTrue();

        refreshRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<RefreshToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task LogoutAsync_DebeLanzarExcepcion_SiTokenNoExiste()
    {
        // Arrange

        var repositoryMock =
            new Mock<IUsuarioRepository>();

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x => x.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        var configurationMock =
            new Mock<IConfiguration>();

        var loggerMock =
            new Mock<ILogger<AuthService>>();

        var service =
            new AuthService(
                repositoryMock.Object,
                refreshRepositoryMock.Object,
                configurationMock.Object,
                loggerMock.Object
            );

        // Act

        Func<Task> accion = async () =>
            await service.LogoutAsync("TOKEN_INEXISTENTE");

        // Assert

        await accion.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Refresh Token Invalido");
    }
}
