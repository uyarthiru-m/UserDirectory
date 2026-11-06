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
}
