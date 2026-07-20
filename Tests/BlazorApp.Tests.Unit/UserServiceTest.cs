using BlazorApp.Models;
using BlazorApp.Services;
using BlazorApp.Data;
using BlazorApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;

namespace BlazorApp.Tests;

public class UserServiceTest
{

    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _service;

    public UserServiceTest()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _service = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task GetUserRoleAsync_UserHasRole_ReturnsRole()
    {
        // Arrange
        var user = new User {
            UserName = "admin"
        };

        _userRepoMock.Setup(m => m.GetUserRoleAsync(user)).ReturnsAsync("Admin");

        //Act
        var result = await _service.GetUserRoleAsync(user);

        //Assert
        Assert.Equal("Admin", result);
    }
}
