using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Repositories
{
    public interface IPlantRepository : IRepository<Plant>
    {
        Task<Plant?> GetPlantByNameAsync(string name);
        Task<List<Plant>> GetAllPlants();
        Task<List<Plant>> GetEnabledPlantsAsync();
        Task<Plant?> GetPlantByIdAsync(int id);
    }

    public class PlantRepository : RepositoryBase<Plant>, IPlantRepository
    {
        public PlantRepository(ApplicationDbContext context) : base(context) {
        }

        public async Task<Plant?> GetPlantByNameAsync(string name) {
            return await _context.Plants.Include(p => p.Users).Include(p => p.Variables).FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Plant>> GetAllPlants() {
            return await _context.Set<Plant>().Include(p => p.Users).Include(p => p.Variables).ToListAsync();
        }

        public async Task<List<Plant>> GetEnabledPlantsAsync()
        {
            return await _context.Plants.Where(p => p.Enabled).Include(p => p.Users).Include(p => p.Variables).ToListAsync();
        }

        public async Task<Plant?> GetPlantByIdAsync(int id) {
            return await _context.Plants.Include(p => p.Users).Include(p => p.Variables).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}