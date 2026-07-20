using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Repositories;

namespace BlazorApp.Services
{
    public class UserService : ServiceBase<User>
    {
        protected readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo) : base(userRepo) {
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetUsersAsync() {
            return await _userRepo.GetUsersAsync();
        }

        public async Task<User?> GetUserAsync(int id) {
            return await _userRepo.GetUserAsync(id);
        }

        public async Task<List<string>> GetAllRolesAsync() {
            return await _userRepo.GetAllRolesAsync();
        }

        public async Task<string> GetUserRoleAsync(User user) {
            return await _userRepo.GetUserRoleAsync(user);
        }

        public async Task<IdentityResult> CreateUserAsync(string name, string username, string email, string password, bool enabled, string role, HashSet<int> selectedPlantIds) {
            return await _userRepo.CreateUserAsync(name, username, email, password, enabled, role, selectedPlantIds);            
        }

        public async Task<IdentityResult> UpdateUserAsync(User user, string role) {
            return await _userRepo.UpdateUserAsync(user, role);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user) {
            return await _userRepo.DeleteUserAsync(user);
        }

        public async Task<IdentityResult> UpdateUserWithPlantsAsync(User user, string role, IEnumerable<int> selectedPlantIds) {
            return await _userRepo.UpdateUserWithPlantsAsync(user, role, selectedPlantIds);
        }
    }
}