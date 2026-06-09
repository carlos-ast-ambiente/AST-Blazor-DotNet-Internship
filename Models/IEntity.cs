using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        bool Enabled { get; set; }
    }
}