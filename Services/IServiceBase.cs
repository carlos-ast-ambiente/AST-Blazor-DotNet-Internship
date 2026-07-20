using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Data;
using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Repositories;

namespace BlazorApp.Services
{
    public interface IServiceBase<T> where T : class, IEntity
    {
        Task<List<T>> GetAllEnabled(bool enabled);
        Task<T> GetById(bool Enabled, int Id);
        Task<T> Insert(T type);
        Task Update(T type);
        Task Delete(int Id);
    }

    public class ServiceBase<T> : IServiceBase<T> where T : class, IEntity
    {
        protected readonly IRepository<T> _repository;
        public ServiceBase(IRepository<T> repository) {
            this._repository = repository;
        }

        public async Task<List<T>> GetAllEnabled(bool enabled) {
            return await _repository.GetAllEnabled(enabled);
        }

        public async Task<List<T>> GetAll() {
            return await _repository.GetAll();
        }

        public async Task<T> GetById(bool Enabled, int Id) {
            return  await _repository.GetById(Enabled, Id);
        }
        public async Task<T> Insert(T type) {
            return await _repository.Insert(type);
        }
        public async Task Update(T type) {
            await _repository.Update(type);
        }
        public async Task Delete(int Id) {
            await _repository.Delete(Id);
        }
    }
}