using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using BlazorApp.Services;
using BlazorApp.Repositories;
using Moq;
using Xunit;
using FluentAssertions;

namespace BlazorApp.Tests.Functional
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _groupRepoMock;
        private readonly GroupService _groupService;

        public GroupServiceTests()
        {
            _groupRepoMock = new Mock<IGroupRepository>();
            _groupService = new GroupService(_groupRepoMock.Object);
        }


        //-------------
        //Create tests
        //-------------
        [Fact]
        public async Task Insert_ShouldReturnCreatedGroup() {
            //Arrange
            var groups = new List<Group>();
            var inputGroup = new Group { Name = "TestGroup" };

            _groupRepoMock.Setup(repo => repo.Insert(It.IsAny<Group>())).Callback<Group>(group => {
                group.Id = 1;
                groups.Add(group);
            }).ReturnsAsync((Group group) => group);

            //Act
            var result = await _groupService.Insert(inputGroup);

            //Assert with fluent assertions
            result.Should().NotBeNull();
            groups.Should().ContainSingle();
            groups.First().Name.Should().Be("TestGroup");
            groups.First().Should().BeEquivalentTo(inputGroup);
        }


        //-------------
        //Read tests
        //-------------

        //GetAllGroups---------------------------------------------

        [Fact]
        //reads a list => returns a list
        public async Task GetAllGroups_ShouldReturnAllGroups() {
            //Arrange
            var expectedGroups = new List<Group> {
                new Group { Id = 1, Name = "Group1" },
                new Group { Id = 2, Name = "Group2" }
            };

            _groupRepoMock.Setup(repo => repo.GetAllGroups()).ReturnsAsync(expectedGroups);

            //Act
            var result = await _groupService.GetAllGroups();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedGroups);

            _groupRepoMock.Verify(repo => repo.GetAllGroups(), Times.Once);
        }

        [Fact]
        //reads list with no groups => returns empty list
        public async Task GetAllGroups_ShouldReturnEmpty() {
            //Arrange
            var emptyList = new List<Group>();
            _groupRepoMock.Setup(repo => repo.GetAllGroups()).ReturnsAsync(emptyList);

            //Act
            var result = await _groupService.GetAllGroups();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(emptyList);
            _groupRepoMock.Verify(repo => repo.GetAllGroups(), Times.Once);
        }


        //GetGroupAsync-----------------------------------------------

        [Fact]
        //reads an id => returns a group
        public async Task GetGroupAsync_ShouldReturnGroupById() {
            //Arrange
            int groupId = 1;
            var expectedGroup = new Group { Id = groupId, Name = "testgroup" };

            _groupRepoMock.Setup(repo => repo.GetGroupAsync(groupId)).ReturnsAsync(expectedGroup);

            //Act
            var result = await _groupService.GetGroupAsync(groupId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedGroup.Id);
            result.Name.Should().Be(expectedGroup.Name);

            _groupRepoMock.Verify(repo => repo.GetGroupAsync(groupId), Times.Once);
        }

        [Fact]
        //reads a missing id => returns null
        public async Task GetGroupAsync_ShouldReturnNull() {
            //Arrange
            int missingId = 6;
            _groupRepoMock.Setup(repo => repo.GetGroupAsync(missingId)).ReturnsAsync((Group?)null);

            //Act
            var result = await _groupService.GetGroupAsync(missingId);

            //Assert
            result.Should().BeNull();
            _groupRepoMock.Verify(repo => repo.GetGroupAsync(missingId), Times.Once);
        } 

        //-------------
        //Update tests
        //-------------
        [Fact]
        public async Task Update_ShouldReturnSuccess() {
            //Arrange
            var updatedGroup = new Group {Id = 1, Name = "upGroup"};
            _groupRepoMock.Setup(repo => repo.Update(updatedGroup)).Returns(Task.CompletedTask);

            //Act
            await _groupService.Update(updatedGroup);

            //Assert
            _groupRepoMock.Verify(repo => repo.Update(updatedGroup), Times.Once);
        }

        //-------------
        //Delete tests
        //-------------
        [Fact]
        public async Task Delete_ShouldDeleteGroup() {
            //Arrange
            var groups = new List<Group> {
                new Group { Id = 1, Name = "Group1" }
            };
            var groupId = 1;

            _groupRepoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Callback<int>(id => {
                groups.RemoveAll(g => g.Id == id);
            });

            //Act
            await _groupService.Delete(groupId);

            //Assert
            groups.Should().BeEmpty();

            _groupRepoMock.Verify(repo => repo.Delete(groupId), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound() {
            //Arrange 
            var missingId = 67;
            _groupRepoMock.Setup(repo => repo.Delete(missingId)).Returns(Task.CompletedTask);

            //Act
            Func<Task> act = () => _groupService.Delete(missingId);

            //Assert
            await act.Should().NotThrowAsync();

            _groupRepoMock.Verify(repo => repo.Delete(missingId), Times.Once);
        }
        
    }
}