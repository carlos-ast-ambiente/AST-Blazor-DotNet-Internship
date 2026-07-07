using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services
{
    public class VariableService : ServiceBase<Variable>
    {
        public VariableService(ApplicationDbContext context) : base(context) {
        }

        public async Task<List<Variable>> GetVariablesAsync() {
            return await _context.Variables.OrderBy(v => v.Name).ToListAsync();
        }

        public async Task<Variable?> GetSingleVarAsync(int id) {
            return await _context.Variables.Include(v => v.Group).Include(v => v.Plants).FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}