using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Models
{
    public class Plant : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Database { get; set; }
        public List<Variable> Variables { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public bool Enabled { get; set; }

        public List<User> Users { get; set; }
    }
}