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

namespace BlazorApp.Tests.Functional {

    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepoMock.Object);
        }

        //--------------
        // Create tests
        //--------------
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

        //only checks if service sends an error, if the repo does.
        [Fact]
        public async Task CreateUserAsyn_SameEmail_ShouldReturnError()
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
        
            var sameEmail = IdentityResult.Failed(new IdentityError{
                Description = "Email already exists."
            });

            _userRepoMock.Setup(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds)).ReturnsAsync(sameEmail);

            //Act
            var result = await _userService.CreateUserAsync(name, username, email, password, enabled, role, plantIds);


            //Assert with fluent assertions
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Description == "Email already exists.");

            _userRepoMock.Verify(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds), Times.Once);
        }

        //only checks if service sends an error, if the repo does.
        [Fact]
        public async Task CreateUserAsyn_SameUsername_ShouldReturnError()
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
        
            var sameUsername = IdentityResult.Failed(new IdentityError{
                Description = "Username already exists."
            });

            _userRepoMock.Setup(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds)).ReturnsAsync(sameUsername);

            //Act
            var result = await _userService.CreateUserAsync(name, username, email, password, enabled, role, plantIds);


            //Assert with fluent assertions
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Description == "Username already exists.");

            _userRepoMock.Verify(repo => repo.CreateUserAsync(name, username, email, password, enabled, role, plantIds), Times.Once);
        }



        //------------
        // Read tests
        //------------

        //GetUserAsync ------------------------------------

        [Fact]
        //reads single user
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
        //missing user => returns null
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

        //GetUsersAsync-------------------------------------------

        [Fact]
        //reads all users => returns list
        public async Task GetUsersAsync_ShouldReturnUsers() {
            //Arrange
            var users = new List<User> {
                new User {Id = 1, Name = "Client1", UserName = "client1"},
                new User {Id = 2, Name = "Client2", UserName = "client2"},
                new User {Id = 3, Name = "Client3", UserName = "client3"}
            };
            _userRepoMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

            //Act
            var result = await _userService.GetUsersAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(users);

            _userRepoMock.Verify(repo => repo.GetUsersAsync(), Times.Once);
        }

        [Fact]
        //missing users => returns empty list
        public async Task GetUsersAsync_ShouldReturnEmpty() {
            //Arrange
            var emptyList = new List<User>();
            _userRepoMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(emptyList);

            //Act
            var result = await _userService.GetUsersAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _userRepoMock.Verify(repo => repo.GetUsersAsync(), Times.Once);
        }

        //GetAllRolesAsync------------------------------------------

        [Fact]
        //reads all roles => returns list
        public async Task GetAllRolesAsync_ShouldReturnRoles() {
            //Arrange
            var roles = new List<string>{ "Admin", "Client" };
            _userRepoMock.Setup(repo => repo.GetAllRolesAsync()).ReturnsAsync(roles);

            //Act
            var result = await _userService.GetAllRolesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(roles);
            _userRepoMock.Verify(repo => repo.GetAllRolesAsync(), Times.Once);
        }

        [Fact]
        //missing roles => returns empty list
        public async Task GetAllRolesAsync_ShouldReturnEmpty() {
            //Arrange
            var emptyList = new List<string>();
            _userRepoMock.Setup(repo => repo.GetAllRolesAsync()).ReturnsAsync(emptyList);

            //Act
            var result = await _userService.GetAllRolesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _userRepoMock.Verify(repo => repo.GetAllRolesAsync(), Times.Once);
        }

        //GetUserRoleAsync----------------------------------------------

        [Fact]
        public async Task GetUserRoleAsync_ShouldReturnRole() {
            //Arrange
            var user = new User {Id = 1, UserName = "admin"};
            _userRepoMock.Setup(repo => repo.GetUserRoleAsync(user)).ReturnsAsync("Admin");

            //Act
            var result = await _userService.GetUserRoleAsync(user);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be("Admin");
            _userRepoMock.Verify(repo => repo.GetUserRoleAsync(user), Times.Once);
        }

        [Fact]
        public async Task GetUserRoleAsync_ShouldReturnNull() {
            //Arrange
            var missingUser = new User();
            _userRepoMock.Setup(repo => repo.GetUserRoleAsync(missingUser)).ReturnsAsync("");

            //Act
            var result = await _userService.GetUserRoleAsync(missingUser);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be("");
            _userRepoMock.Verify(repo => repo.GetUserRoleAsync(missingUser), Times.Once);
        }

        //-------------
        //Update tests
        //-------------
        [Fact]
        public async Task UpdateUserAsync_ShouldReturnSuccess() {
            //Arrange
            var updatedUser = new User {
                Id = 3,
                UserName = "updatedUser"
            };

            _userRepoMock.Setup(repo => repo.UpdateUserAsync(updatedUser, "Client")).ReturnsAsync(IdentityResult.Success);

            //Act
            var result = await _userService.UpdateUserAsync(updatedUser, "Client");

            //Assert with fluent assertions
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeEmpty();

            _userRepoMock.Verify(repo => repo.UpdateUserAsync(updatedUser, "Client"), Times.Once);
        } 
        // can't specifically change something

        [Fact]
        public async Task UpdateUserAsync_ShouldNotChangeId() {
            //Arrange
            var updatedUser = new User {
                Id = 3,
                UserName = "updatedUser"
            };

            _userRepoMock.Setup(repo => repo.UpdateUserAsync(updatedUser, "Client")).ReturnsAsync(IdentityResult.Success);

            //Act
            var result = await _userService.UpdateUserAsync(updatedUser, "Client");

            //Assert with fluent assertions
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            updatedUser.Id.Should().Be(3);

            _userRepoMock.Verify(repo => repo.UpdateUserAsync(updatedUser, "Client"), Times.Once);
        } 


        //------------
        //Delete tests
        //------------
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
}