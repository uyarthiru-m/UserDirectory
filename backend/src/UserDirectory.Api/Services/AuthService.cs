using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UserDirectory.Api.Data;
using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;

namespace UserDirectory.Api.Services;

public sealed class AuthService : IAuthService
{
    private const int SqliteUniqueConstraintExtendedCode = 2067;
    private readonly AppDbContext _database;
    private readonly PasswordHasher<LocalAccount> _passwordHasher;
    private readonly IJwtTokenService _tokenService;

    public AuthService(
        AppDbContext database,
        PasswordHasher<LocalAccount> passwordHasher,
        IJwtTokenService tokenService)
    {
        _database = database;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LocalAuthResponse> RegisterAsync(
        LocalAuthRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        var normalizedEmail = NormalizeEmail(email);
        if (await _database.LocalAccounts.AnyAsync(
                account => account.NormalizedEmail == normalizedEmail,
                cancellationToken))
        {
            throw new DuplicateAccountException();
        }

        var account = new LocalAccount
        {
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = string.Empty,
            CreatedAtUtc = DateTime.UtcNow
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, request.Password);
        _database.LocalAccounts.Add(account);

        try
        {
            await _database.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SqliteUniqueConstraintExtendedCode
            })
        {
            throw new DuplicateAccountException();
        }

        return _tokenService.CreateToken(account);
    }

    public async Task<LocalAuthResponse?> LoginAsync(
        LocalAuthRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var account = await _database.LocalAccounts.SingleOrDefaultAsync(
            candidate => candidate.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (account is null
            || _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return _tokenService.CreateToken(account);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();
}
