using Moq;
using UserDirectory.Api.DTOs;
using UserDirectory.Api.Models;
using UserDirectory.Api.Repositories;
using UserDirectory.Api.Services;
using Xunit;

namespace UserDirectory.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _service = new UserService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedDtoList()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Alice Smith", Age = 30, City = "Seattle", State = "WA", Pincode = "98101" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        Assert.Single(result);
        Assert.Equal("Alice Smith", result.First().Name);
    }

    [Fact]
    public async Task CreateUserAsync_PersistsUserAndReturnsDto()
    {
        var dto = new CreateUserDto("Bob Jones", 25, "Austin", "TX", "73301");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => { u.Id = 10; return u; });

        var result = await _service.CreateUserAsync(dto);

        Assert.Equal(10, result.Id);
        Assert.Equal("Bob Jones", result.Name);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsMappedDtoWhenUserExists()
    {
        _repoMock.Setup(repository => repository.GetByIdAsync(7))
            .ReturnsAsync(new User { Id = 7, Name = "Alice", Age = 30, City = "Seattle", State = "WA", Pincode = "98101" });

        var result = await _service.GetUserByIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("Seattle", result.City);
        Assert.Equal("WA", result.State);
        Assert.Equal("98101", result.Pincode);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNullWhenUserDoesNotExist()
    {
        _repoMock.Setup(repository => repository.GetByIdAsync(7)).ReturnsAsync((User?)null);

        var result = await _service.GetUserByIdAsync(7);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesExistingUser()
    {
        var user = new User { Id = 7, Name = "Alice", Age = 30, City = "Seattle", State = "WA", Pincode = "98101" };
        _repoMock.Setup(repository => repository.GetByIdAsync(7)).ReturnsAsync(user);
        var update = new UpdateUserDto("Alice Smith", 31, "Portland", "OR", "97201");

        var result = await _service.UpdateUserAsync(7, update);

        Assert.True(result);
        Assert.Equal("Alice Smith", user.Name);
        Assert.Equal(31, user.Age);
        _repoMock.Verify(repository => repository.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFalseWhenUserDoesNotExist()
    {
        _repoMock.Setup(repository => repository.GetByIdAsync(7)).ReturnsAsync((User?)null);

        var result = await _service.UpdateUserAsync(
            7,
            new UpdateUserDto("Alice Smith", 31, "Portland", "OR", "97201"));

        Assert.False(result);
        _repoMock.Verify(repository => repository.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesExistingUser()
    {
        var user = new User { Id = 7, Name = "Alice", Age = 30, City = "Seattle", State = "WA", Pincode = "98101" };
        _repoMock.Setup(repository => repository.GetByIdAsync(7)).ReturnsAsync(user);

        var result = await _service.DeleteUserAsync(7);

        Assert.True(result);
        _repoMock.Verify(repository => repository.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsFalseWhenUserDoesNotExist()
    {
        _repoMock.Setup(repository => repository.GetByIdAsync(7)).ReturnsAsync((User?)null);

        var result = await _service.DeleteUserAsync(7);

        Assert.False(result);
        _repoMock.Verify(repository => repository.DeleteAsync(It.IsAny<User>()), Times.Never);
    }
}
