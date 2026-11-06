using System.ComponentModel.DataAnnotations;

namespace UserDirectory.Api.DTOs;

public record CreateUserDto(
    [Required][StringLength(100, MinimumLength = 2)] string Name,
    [Range(0, 120)] int Age,
    [Required] string City,
    [Required] string State,
    [Required][StringLength(10, MinimumLength = 4)] string Pincode
);

public record UpdateUserDto(
    [Required][StringLength(100, MinimumLength = 2)] string Name,
    [Range(0, 120)] int Age,
    [Required] string City,
    [Required] string State,
    [Required][StringLength(10, MinimumLength = 4)] string Pincode
);

public record UserResponseDto(int Id, string Name, int Age, string City, string State, string Pincode);
