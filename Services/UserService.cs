using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Services
{
    public class UserService : ServiceBase<User>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager) : base(context) {
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
                    // Fetch the plants using the active context
                    var dbPlants = await _context.Plants
                        .Where(p => selectedPlantIds.Contains(p.Id))
                        .ToListAsync();

                    // Explicitly load the User's Plants collection if it's not initialized
                    user.Plants ??= new List<Plant>();

                    foreach (var plant in dbPlants) {
                        user.Plants.Add(plant);
                    }

                    // Save the newly created relationships to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // If mapping plants fails, we should still return a success for the user,
                    // but log or handle the error so your UI doesn't silently freeze.
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