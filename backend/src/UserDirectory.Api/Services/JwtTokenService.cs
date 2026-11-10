using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;

namespace UserDirectory.Api.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(45);
    private readonly JwtOptions _options;

    public JwtTokenService(JwtOptions options) => _options = options;

    public LocalAuthResponse CreateToken(LocalAccount account)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.Add(TokenLifetime);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LocalAuthResponse(new JwtSecurityTokenHandler().WriteToken(token), account.Email, expiresAt);
    }
}
