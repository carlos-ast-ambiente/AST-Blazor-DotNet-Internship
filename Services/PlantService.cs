using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;


namespace BlazorApp.Services
{
    public class PlantService : ServiceBase<Plant>
    {
        /*public override async Task<List<Plant>> GetAllEnabled(bool enabled) {

        } */
        public PlantService(ApplicationDbContext context) : base(context) {
        }
    }
}