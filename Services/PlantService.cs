using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using BlazorApp.Repositories;

namespace BlazorApp.Services
{
    public class PlantService : ServiceBase<Plant>
    {
        /*public override async Task<List<Plant>> GetAllEnabled(bool enabled) {

        } */
        protected readonly IPlantRepository _plantRepository;

        public PlantService(IPlantRepository plantRepository) : base(plantRepository) {
            _plantRepository = plantRepository;
        }

        public async Task<Plant?> GetPlantByNameAsync(string name) {
            return await _plantRepository.GetPlantByNameAsync(name);
        }

        public async Task<List<Plant>> GetAllPlants() {
            return await _plantRepository.GetAllPlants();
        }

        public async Task<List<Plant>> GetEnabledPlantsAsync()
        {
            return await _plantRepository.GetEnabledPlantsAsync();
        }
    }
}