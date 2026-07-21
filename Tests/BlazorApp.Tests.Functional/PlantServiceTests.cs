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
    public class PlantServiceTests
    {
        private readonly Mock<IPlantRepository> _plantRepoMock;
        private readonly PlantService _plantService;

        public PlantServiceTests()
        {
            _plantRepoMock = new Mock<IPlantRepository>();
            _plantService = new PlantService(_plantRepoMock.Object);
        }

        //-------------
        //Create tests
        //-------------
        [Fact]
        public async Task Insert_ShouldReturnCreatedPlant() {
            //Arrange
            var plants = new List<Plant>();
            var inputPlant = new Plant { Name = "TestPlant" };

            _plantRepoMock.Setup(repo => repo.Insert(It.IsAny<Plant>())).Callback<Plant>(plant => {
                plant.Id = 1;
                plants.Add(plant);
            }).ReturnsAsync((Plant plant) => plant);

            //Act
            var result = await _plantService.Insert(inputPlant);

            //Assert with Fluent Assertions 
            result.Should().NotBeNull();
            plants.Should().ContainSingle();
            plants.Single().Name.Should().Be("TestPlant");
        }


        //-------------
        //Read tests
        //-------------

        //GetPlantByNameAsync-------------------------------------------

        [Fact]
        //reads name => returns plant
        public async Task GetPlantByNameAsync_ShouldReturnPlant(){
            //Arrange
            string name = "TestPlant";
            var expectedPlant = new Plant {Id = 8, Name = name};

            _plantRepoMock.Setup(repo => repo.GetPlantByNameAsync(name)).ReturnsAsync(expectedPlant);

            //Act 
            var result = await _plantService.GetPlantByNameAsync(name);

            //Assert with fluent assertions 
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPlant);
            result.Id.Should().Be(expectedPlant.Id);

            _plantRepoMock.Verify(repo => repo.GetPlantByNameAsync(name), Times.Once);
        }

        [Fact]
        //reads name => returns null
        public async Task GetPlantByNameAsync_ShouldReturnNull() {
            //Arrange
            string name = "Missing";

            _plantRepoMock.Setup(repo => repo.GetPlantByNameAsync(name)).ReturnsAsync((Plant?)null);

            //Act
            var result = await _plantService.GetPlantByNameAsync(name);

            //Assert
            result.Should().BeNull();
            _plantRepoMock.Verify(repo => repo.GetPlantByNameAsync(name), Times.Once);
        }

        //GetAllPlants---------------------------------------------

        [Fact]
        //reads list => returns list
        public async Task GetAllPlants_ShouldReturnAllPlants() {
            //Arrange
            var expectedPlants = new List<Plant> {
                new Plant {Id = 1, Name = "Plant1"},
                new Plant {Id = 2, Name = "Plant2"},
                new Plant {Id = 3, Name = "Plant3"}
            };

            _plantRepoMock.Setup(repo => repo.GetAllPlants()).ReturnsAsync(expectedPlants);

            //Act
            var result = await _plantService.GetAllPlants();

            //Assert with fluent assertions
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPlants);

            _plantRepoMock.Verify(repo => repo.GetAllPlants(), Times.Once);
        }

        [Fact]
        //reads list with no plants => returns empty list
        public async Task GetAllPlants_ShouldReturnEmpty(){
            //Arrange
            var emptyList = new List<Plant>();
            _plantRepoMock.Setup(repo => repo.GetAllPlants()).ReturnsAsync(emptyList);

            //Act
            var result = await _plantService.GetAllPlants();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _plantRepoMock.Verify(repo => repo.GetAllPlants(), Times.Once);
        }

        //GetEnabledPlantsAsync-------------------------------------------------

        [Fact]
        //reads list => returns list of enabled
        public async Task GetEnabledPlantsAsync_ShouldReturnEnabledPlants() { 
            //Arrange
            var allPlants = new List<Plant> {
                new Plant {Id = 1, Name = "Plant1", Enabled = true},
                new Plant {Id = 2, Name = "Plant2", Enabled = false},
                new Plant {Id = 3, Name = "Plant3", Enabled = true},
                new Plant {Id = 4, Name = "Plant4", Enabled = true}
            };

            var expectedPlants = allPlants.Where(p => p.Enabled).ToList();

            _plantRepoMock.Setup(repo => repo.GetEnabledPlantsAsync()).ReturnsAsync(expectedPlants);

            //Act 
            var result = await _plantService.GetEnabledPlantsAsync();

            //Assert with fluent assertions
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPlants);

            _plantRepoMock.Verify(repo => repo.GetEnabledPlantsAsync(), Times.Once);
        }

        [Fact]
        //reads list with no plants => returns empty list
        public async Task GetEnabledPlantsAsync_NoPlants_ShouldReturnEmpty() {
            //Arrange
            var emptyList = new List<Plant>();
            _plantRepoMock.Setup(repo => repo.GetEnabledPlantsAsync()).ReturnsAsync(emptyList);

            //Act
            var result = await _plantService.GetEnabledPlantsAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _plantRepoMock.Verify(repo => repo.GetEnabledPlantsAsync(), Times.Once);
        }

        [Fact]
        //reads list with disabled plants => returns empty list
        public async Task GetEnabledPlantsAsync_ShouldReturnEmpty() {
            //Arrange
            var plants = new List<Plant> {
                new Plant {Id = 1, Name = "Plant1", Enabled = false}, 
                new Plant {Id = 2, Name = "Plant2", Enabled = false}, 
                new Plant {Id = 3, Name = "Plant3", Enabled = false} 
            };
            var emptyList = new List<Plant>();

            _plantRepoMock.Setup(repo => repo.GetEnabledPlantsAsync()).ReturnsAsync(emptyList);

            //Act
            var result = await _plantService.GetEnabledPlantsAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _plantRepoMock.Verify(repo => repo.GetEnabledPlantsAsync(), Times.Once);
        }


        //-------------
        //Update tests
        //-------------
        [Fact]
        public async Task Update_ShouldReturnSuccess() {
            //Arrange
            var updatedPlant = new Plant {Id = 1, Name = "upPlant"};
            _plantRepoMock.Setup(repo => repo.Update(updatedPlant)).Returns(Task.CompletedTask);

            //Act
            await _plantService.Update(updatedPlant);

            //Assert
            _plantRepoMock.Verify(repo => repo.Update(updatedPlant), Times.Once);
        }

        //-------------
        //Delete tests
        //-------------
        [Fact]
        public async Task Delete_ShouldDeletePlant() {
            //Arrange
            var plants = new List<Plant> {
                new Plant { Id = 5, Name = "PlantToDelete"}
            };
            var plantId = 5;
            
            _plantRepoMock.Setup(repo => repo.Delete(It.IsAny<int>())).Callback<int>(id => {
                plants.RemoveAll(p => p.Id == id);
            });

            //Act
            await _plantService.Delete(plantId);

            //Assert with fluent assertions
            plants.Should().BeEmpty();

            _plantRepoMock.Verify(repo => repo.Delete(plantId), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound(){
            //Arrange
            var missingId = 99;
            _plantRepoMock.Setup(repo => repo.Delete(missingId)).Returns(Task.CompletedTask);
            
            //Act 
            Func<Task> act = () => _plantService.Delete(missingId);

            //Assert
            await act.Should().NotThrowAsync();

            _plantRepoMock.Verify(repo => repo.Delete(missingId), Times.Once);
        }

    }
}