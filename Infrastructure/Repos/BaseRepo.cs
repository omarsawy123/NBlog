using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class BaseRepo<T, TKey> : IBaseRepo<T, TKey> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BaseRepo<T, TKey>> _logger;

        // Inject ILogger via constructor
        public BaseRepo(ApplicationDbContext context, ILogger<BaseRepo<T, TKey>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entities of type {Type}.", typeof(T).Name);
                throw; 
            }
        }

        public IQueryable<T> GetAll()
        {
            try
            {
                return _context.Set<T>().AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all entities of type {Type} as IQueryable.", typeof(T).Name);
                throw; 
            }
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            try
            {
                return await _context.Set<T>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching an entity of type {Type} with ID {Id}.", typeof(T).Name, id);
                throw; 
            }
        }

        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding an entity of type {Type}.", typeof(T).Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating an entity of type {Type}.", typeof(T).Name);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting an entity of type {Type}.", typeof(T).Name);
                throw; 
            }
        }
    }
}