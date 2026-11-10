using UserDirectory.Api.DTOs;

namespace UserDirectory.Api.Services;

public interface IAuthService
{
    Task<LocalAuthResponse> RegisterAsync(LocalAuthRequest request, CancellationToken cancellationToken = default);
    Task<LocalAuthResponse?> LoginAsync(LocalAuthRequest request, CancellationToken cancellationToken = default);
}
