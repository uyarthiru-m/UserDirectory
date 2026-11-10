using System.ComponentModel.DataAnnotations;

namespace UserDirectory.Api.DTOs;

public record LocalAuthRequest(
    [Required][EmailAddress][StringLength(254)] string Email,
    [Required][StringLength(128, MinimumLength = 12)] string Password
);

public record LocalAuthResponse(string AccessToken, string Email, DateTime ExpiresAtUtc);
