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
    public class VariableServiceTests
    {
        private readonly Mock<IVariableRepository> _variableRepoMock;
        private readonly VariableService _variableService;

        public VariableServiceTests()
        {
            _variableRepoMock = new Mock<IVariableRepository>();
            _variableService = new VariableService(_variableRepoMock.Object);
        }

        //Create tests
        [Fact]
        public async Task Insert_ShouldReturnCreatedVariable() {
            //Arrange
            var variables = new List<Variable>();
            var inputvariable = new Variable { Name = "Testvariable" };

            _variableRepoMock.Setup(repo => repo.Insert(It.IsAny<Variable>())).Callback<Variable>(variable => {
                variable.Id = 1;
                variables.Add(variable);
            }).ReturnsAsync((Variable variable) => variable);

            //Act
            var result = await _variableService.Insert(inputvariable);

            //Assert with fluent assertions
            result.Should().NotBeNull();
            variables.Should().ContainSingle();
            variables.First().Name.Should().Be("Testvariable");
        }

        //Read tests
        [Fact]
        public async Task GetVariablesAsync_ShouldReturnAllVariables() {
            //Arrange
            var expectedvariables = new List<Variable> {
                new Variable { Id = 1, Name = "Variable1" },
                new Variable { Id = 2, Name = "Variable2" }
            };

            _variableRepoMock.Setup(repo => repo.GetVariablesAsync()).ReturnsAsync(expectedvariables);

            //Act
            var result = await _variableService.GetVariablesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedvariables);

            _variableRepoMock.Verify(repo => repo.GetVariablesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSingleVarAsync_ShouldReturnVariableById() {
            //Arrange
            int variableId = 1;
            var expectedvariable = new Variable { Id = variableId, Name = "testvariable" };

            _variableRepoMock.Setup(repo => repo.GetSingleVarAsync(variableId)).ReturnsAsync(expectedvariable);

            //Act
            var result = await _variableService.GetSingleVarAsync(variableId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedvariable.Id);
            result.Name.Should().Be(expectedvariable.Name);

            _variableRepoMock.Verify(repo => repo.GetSingleVarAsync(variableId), Times.Once);
        }

        //Update tests

        //Delete tests
        [Fact]
        public async Task Delete_ShouldDeleteVariable() {
            //Arrange
            var variables = new List<Variable> {
                new Variable { Id = 1, Name = "variable1" }
            };
            var variableId = 1;

            _variableRepoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Callback<int>(id => {
                variables.RemoveAll(v => v.Id == id);
            });

            //Act
            await _variableService.Delete(variableId);

            //Assert
            variables.Should().BeEmpty();

            _variableRepoMock.Verify(repo => repo.Delete(variableId), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound() {
            //Arrange 
            var missingId = 67;
            _variableRepoMock.Setup(repo => repo.Delete(missingId)).Returns(Task.CompletedTask);

            //Act
            Func<Task> act = () => _variableService.Delete(missingId);

            //Assert
            await act.Should().NotThrowAsync();

            _variableRepoMock.Verify(repo => repo.Delete(missingId), Times.Once);
        }
    }
}