using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class UserService : ServiceBase<User>
    {
        public UserService(ApplicationDbContext context) : base(context) {
        }
    }
}