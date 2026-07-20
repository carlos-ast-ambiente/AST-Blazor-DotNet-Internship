using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using BlazorApp.Repositories;

namespace BlazorApp.Services
{
    public class VariableService : ServiceBase<Variable>
    {
        protected readonly IVariableRepository _varRepository;

        public VariableService(IVariableRepository varRepository) : base(varRepository) {
            _varRepository = varRepository;
        }

        public async Task<List<Variable>> GetVariablesAsync() {
            return await _varRepository.GetVariablesAsync();
        }

        public async Task<Variable?> GetSingleVarAsync(int id) {
            return await _varRepository.GetSingleVarAsync(id);
        }
    }
}