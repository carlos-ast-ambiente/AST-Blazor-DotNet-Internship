using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Models;
using BlazorApp.Data;
using BlazorApp.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllEnabled(bool enabled);
        Task<List<T>> GetAll();
        Task<T> GetById(bool Enabled, int Id);
        Task<T> Insert(T type);
        Task Update(T type);
        Task Delete(int Id);

        Task SaveChangesAsync();
    }

    
    public class RepositoryBase<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly ApplicationDbContext _context;
        public RepositoryBase(ApplicationDbContext context) {
            this._context = context;
        }

        public async Task<List<T>> GetAllEnabled(bool enabled) {
            return await _context.Set<T>()
            .Where(x => x.Enabled == enabled)
            .ToListAsync();
        }

        public async Task<List<T>> GetAll() {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(bool Enabled, int Id) {
            var dev = await _context.Set<T>().FirstOrDefaultAsync(y=> y.Enabled == true && y.Id == Id);
            return dev;
        }
        public async Task<T> Insert(T type) {
            _context.Add(type);
            await _context.SaveChangesAsync();
            return type; // ??
        }
        public async Task Update(T type) {
            _context.Entry(type).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int Id) {
            var dev = await _context.Set<T>().FindAsync(Id);
            if (dev == null) return;
            _context.Set<T>().Remove(dev);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() {
            await _context.SaveChangesAsync();
        }
    }
}