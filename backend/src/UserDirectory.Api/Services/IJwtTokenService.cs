using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;

namespace UserDirectory.Api.Services;

public interface IJwtTokenService
{
    LocalAuthResponse CreateToken(LocalAccount account);
}
