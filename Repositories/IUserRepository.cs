using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserAsync(int id);
        Task<List<string>> GetAllRolesAsync();
        Task<string> GetUserRoleAsync(User user);
        Task<IdentityResult> CreateUserAsync(string name, string username, string email, string password, bool enabled, string role, HashSet<int> selectedPlantIds);
        Task<IdentityResult> UpdateUserAsync(User user, string role);
        Task<IdentityResult> DeleteUserAsync(User user);
        Task<IdentityResult> UpdateUserWithPlantsAsync(User user, string role, IEnumerable<int> selectedPlantIds);

    }


    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager) : base(context) {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<User>> GetUsersAsync() {
            return await _userManager.Users.Include(u => u.Plants.Where(p => p.Enabled)).OrderBy(u => u.UserName).ToListAsync();
        }

        public async Task<User?> GetUserAsync(int id) {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<string>> GetAllRolesAsync() {
            return await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();
        }

        public async Task<string> GetUserRoleAsync(User user) {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "No Role";
        }

        public async Task<IdentityResult> CreateUserAsync(string name, string username, string email, string password, bool enabled, string role, HashSet<int> selectedPlantIds) {
            var user = new User {
                Name = name,
                UserName = username,
                Email = email,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow, 
                Enabled = enabled
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded) {
                return result;
            }

            if (selectedPlantIds != null && selectedPlantIds.Any()) {
                try {
                    var dbPlants = await _context.Plants
                        .Where(p => selectedPlantIds.Contains(p.Id))
                        .ToListAsync();

                    user.Plants ??= new List<Plant>();

                    foreach (var plant in dbPlants) {
                        user.Plants.Add(plant);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error associating plants during user creation: {ex.Message}");
                }
            }

            return await _userManager.AddToRoleAsync(user, role);            
        }

        public async Task<IdentityResult> UpdateUserAsync(User user, string role) {
            user.DateUpdated = DateTime.UtcNow;
            user.NormalizedUserName = user.UserName?.ToUpperInvariant();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result;

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!currentRoles.Contains(role))
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                    return removeResult;

                var addResult = await _userManager.AddToRoleAsync(user, role);

                if (!addResult.Succeeded)
                    return addResult;
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUserAsync(User user) {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> UpdateUserWithPlantsAsync(User user, string role, IEnumerable<int> selectedPlantIds) {
            var dbPlants = await _context.Plants.Where(p => selectedPlantIds.Contains(p.Id)).ToListAsync();

            user.Plants.Clear();
            foreach (var plant in dbPlants) {
                user.Plants.Add(plant);
            }

            return await UpdateUserAsync(user, role);
        }
    }
}