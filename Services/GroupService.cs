using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;

namespace BlazorApp.Services
{
    public class GroupService : ServiceBase<Group>
    {
        public GroupService(ApplicationDbContext context) : base(context) {
        }
    }
}