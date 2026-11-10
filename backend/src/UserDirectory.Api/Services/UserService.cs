using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;
using UserDirectory.Api.Repositories;

namespace UserDirectory.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) => _repository = repository;

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(MapUser);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? null : MapUser(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Age = dto.Age,
            City = dto.City,
            State = dto.State,
            Pincode = dto.Pincode
        };
        var created = await _repository.AddAsync(user);
        return MapUser(created);
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return false;

        user.Name = dto.Name;
        user.Age = dto.Age;
        user.City = dto.City;
        user.State = dto.State;
        user.Pincode = dto.Pincode;

        await _repository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return false;

        await _repository.DeleteAsync(user);
        return true;
    }

    private static UserResponseDto MapUser(User user) =>
        new(user.Id, user.Name, user.Age, user.City, user.State, user.Pincode);
}