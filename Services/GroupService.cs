using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services
{
    public class GroupService : ServiceBase<Group>
    {
        public GroupService(ApplicationDbContext context) : base(context) {
        }

        public async Task<List<Group>> GetAllGroups() {
            return await _context.Set<Group>().Include(g => g.Variables.Where(v => v.Enabled))
            .ToListAsync();
        }

        public async Task<Group?> GetGroupAsync(int id) {
            return await _context.Groups.Include(g => g.Variables.Where(v => v.Enabled)).FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}