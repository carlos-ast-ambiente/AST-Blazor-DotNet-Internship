using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Services;
using BlazorApp.Repositories;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using FluentAssertions;

namespace BlazorApp.Tests.Functional;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepoMock.Object);
    }

    // Create tests
    [Fact]
    public async Task CreateUserAsync_ShouldReturnCreatedUser()
    {
        // Arrange
        var users = new List<User>();

        string username = "client10";
        string email = "client10@example.com";
        string name = "Client10";
        string password = "1234aZ.";
        bool enabled = true;
        string role = "Client";
        var plantIds = new HashSet<int> { 1, 2, 3 };

        var fakeIdentityResult = IdentityResult.Success;

        _userRepoMock.Setup(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds)).Callback(() => { users.Add(new User {
            Id = 1,
            UserName = username,
            Email = email
        });}).ReturnsAsync(fakeIdentityResult);


        //Act
        var result = await _userService.CreateUserAsync(name, username, email, password, enabled, role, plantIds);


        //Assert with fluent assertions
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        users.Should().ContainSingle();
        users[0].UserName.Should().Be(username);
        users[0].Email.Should().Be(email);

        _userRepoMock.Verify(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds), Times.Once);
    }


    // Read tests
    [Fact]
    public async Task GetUserAsync_ShouldReturnUser()
    {
        // Arrange
        int userId = 1;
        var expectedUser = new User { Id = userId, UserName = "testuser" };

        _userRepoMock.Setup(repo => repo.GetUserAsync(userId)).ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserAsync(userId);

        // Assert with fluent assertions
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedUser.Id);
        result.UserName.Should().Be(expectedUser.UserName);

        _userRepoMock.Verify(repo => repo.GetUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserAsync_ShouldReturnNull() {
        //Arrange 
        int missingId = 7;
        _userRepoMock.Setup(repo => repo.GetUserAsync(missingId)).ReturnsAsync((User?)null);

        //Act
        var result = await _userService.GetUserAsync(missingId);

        //Assert with fluent assertions
        result.Should().BeNull();
        _userRepoMock.Verify(repo => repo.GetUserAsync(missingId), Times.Once);
    }

    //Update tests
    /* [Fact]
    public async Task UpdateUserAsync_ShouldReturnUpdatedUser() {
        //Arrange
        var users = new List<User> {
            new User { Id = 3, UserName = "client3" }
        };

        var updatedUser = new User {
            Id = 3,
            UserName = "updatedUser"
        };

        _userRepoMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>(), It.IsAny<string>())).Callback<User, string>((user, role) => {
            var dbUser = users.Single(u => u.Id == user.Id);
            dbUser.UserName =  user.UserName;
        }).ReturnsAsync(IdentityResult.Success);

        //Act
        var result = await _userService.UpdateUserAsync(updatedUser, "Client");

        //Assert with fluent assertions
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        users.Single().UserName.Should().Be("updatedUser");

        _userRepoMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    } */
    // can't specifically change something


    //Delete tests
    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser() {
        //Arrange
        var users = new List<User> {
            new User { Id = 5, UserName = "client5"}
        };
        var userToDelete = users.Single();
        var fakeIdentityResult = IdentityResult.Success;

        _userRepoMock.Setup(repo => repo.DeleteUserAsync(It.IsAny<User>())).Callback<User>(user => {
            users.RemoveAll(u => u.Id == user.Id);
        }).ReturnsAsync(fakeIdentityResult);


        //Act
        var result = await _userService.DeleteUserAsync(userToDelete);

        //Assert with fluent assertions
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();

        _userRepoMock.Verify(repo => repo.DeleteUserAsync(userToDelete), Times.Once);
    }
}
