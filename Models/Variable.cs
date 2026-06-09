using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Models
{
    public class Variable : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Table { get; set; }
        public string? Column { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? Unit { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool Enabled { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        public List<Plant> Plants { get; set; }
    }
}