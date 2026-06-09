using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services
{
    public class VariableService : ServiceBase<Variable>
    {
        public VariableService(ApplicationDbContext context) : base(context) {
        }
    }
}