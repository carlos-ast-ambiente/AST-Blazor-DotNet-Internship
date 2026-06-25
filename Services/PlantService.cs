using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;


namespace BlazorApp.Services
{
    public class PlantService : ServiceBase<Plant>
    {
        /*public override async Task<List<Plant>> GetAllEnabled(bool enabled) {

        } */
        public PlantService(ApplicationDbContext context) : base(context) {
        }

        public async Task<Plant?> GetPlantByNameAsync(string name) {
            return await _context.Plants.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Plant>> GetAllPlants() {
            return await _context.Set<Plant>()
            .ToListAsync();
        }
    }
}