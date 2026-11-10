using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserDirectory.Api.Data;
using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;
using UserDirectory.Api.Services;

namespace UserDirectory.Tests;

public sealed class AuthServiceTests : IDisposable
{
    private const string Password = "A-secure-test-password-123";
    private const string SigningKey = "test-signing-key-that-is-at-least-32-bytes-long";
    private const string Issuer = "UserDirectory.Tests";
    private const string Audience = "UserDirectory.Tests.Client";

    private readonly SqliteConnection _connection;
    private readonly AppDbContext _database;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _database = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options);
        _database.Database.EnsureCreated();

        var tokenService = new JwtTokenService(new JwtOptions(Issuer, Audience, SigningKey));
        _service = new AuthService(_database, new PasswordHasher<LocalAccount>(), tokenService);
    }

    [Fact]
    public async Task RegisterAsync_CreatesNormalizedAccountWithHashedPasswordAndValidToken()
    {
        var result = await _service.RegisterAsync(new LocalAuthRequest("  Alice@example.com  ", Password));

        var account = await _database.LocalAccounts.SingleAsync();
        Assert.Equal("Alice@example.com", result.Email);
        Assert.Equal("ALICE@EXAMPLE.COM", account.NormalizedEmail);
        Assert.NotEqual(Password, account.PasswordHash);
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);

        var principal = new JwtSecurityTokenHandler { MapInboundClaims = false }.ValidateToken(
            result.AccessToken,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
                ClockSkew = TimeSpan.Zero
            },
            out _);

        Assert.Equal(account.Id.ToString(), principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
        Assert.Equal("Alice@example.com", principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
    }

    [Fact]
    public async Task RegisterAsync_RejectsDuplicateEmailRegardlessOfCase()
    {
        await _service.RegisterAsync(new LocalAuthRequest("Alice@example.com", Password));

        await Assert.ThrowsAsync<DuplicateAccountException>(() =>
            _service.RegisterAsync(new LocalAuthRequest(" alice@EXAMPLE.com ", Password)));

        Assert.Single(await _database.LocalAccounts.ToListAsync());
    }

    [Fact]
    public async Task LoginAsync_AcceptsCaseInsensitiveEmailAndCorrectPassword()
    {
        await _service.RegisterAsync(new LocalAuthRequest("Alice@example.com", Password));

        var result = await _service.LoginAsync(new LocalAuthRequest(" ALICE@EXAMPLE.COM ", Password));

        Assert.NotNull(result);
        Assert.Equal("Alice@example.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
    }

    [Theory]
    [InlineData("alice@example.com", "wrong-password-123")]
    [InlineData("unknown@example.com", Password)]
    public async Task LoginAsync_ReturnsNullForInvalidCredentials(string email, string password)
    {
        await _service.RegisterAsync(new LocalAuthRequest("alice@example.com", Password));

        var result = await _service.LoginAsync(new LocalAuthRequest(email, password));

        Assert.Null(result);
    }

    public void Dispose()
    {
        _database.Dispose();
        _connection.Dispose();
    }
}
