namespace UserDirectory.Api.Services;

public sealed record JwtOptions(string Issuer, string Audience, string SigningKey);
