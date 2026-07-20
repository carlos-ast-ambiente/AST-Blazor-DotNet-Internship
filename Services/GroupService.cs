using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Repositories;

namespace BlazorApp.Services
{
    public class GroupService : ServiceBase<Group>
    {
        protected readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository) : base(groupRepository) {
            _groupRepository = groupRepository;
        }

        public async Task<List<Group>> GetAllGroups() {
            return await _groupRepository.GetAllGroups();
        }

        public async Task<Group?> GetGroupAsync(int id) {
            return await _groupRepository.GetGroupAsync(id);
        }
    }
}