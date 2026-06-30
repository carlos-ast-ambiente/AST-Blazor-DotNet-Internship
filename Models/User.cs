using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp.Models
{
    public class User : IdentityUser<int>, IEntity
    {
        // public int Id { get; set; }
        public string Name { get; set; }
        public List<Plant> Plants { get; set; }
        // public List<string> Roles { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Enabled { get; set; }
    }
}