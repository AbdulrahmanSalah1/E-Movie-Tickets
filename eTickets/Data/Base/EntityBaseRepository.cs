using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eTickets.Data.Base
{
    // Generic repository class that provides CRUD operations for entities
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        // The database context used for interacting with the database
        private readonly AppDbContext _context;

        // Constructor that initializes the repository with the provided database context
        public EntityBaseRepository(AppDbContext context)
        {
            _context = context;
        }

        // Adds a new entity to the database asynchronously
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);  // Adds the entity to the DbSet
            await _context.SaveChangesAsync();  // Saves the changes to the database
        }

        // Deletes an entity by its ID from the database asynchronously
        public async Task DeleteAsync(int id)
        {
            // Finds the entity by its ID
            var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

            // Marks the entity as deleted
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Deleted;

            // Saves the changes to the database
            await _context.SaveChangesAsync();
        }

        // Retrieves all entities from the database asynchronously
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        // Retrieves all entities with related entities included, based on the specified properties
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            // Builds a query that includes the related properties
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            // Executes the query and returns the result
            return await query.ToListAsync();
        }

        // Retrieves an entity by its ID from the database asynchronously
        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

        // Retrieves an entity by its ID with related entities included, based on the specified properties
        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            // Builds a query that includes the related properties
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            // Executes the query and returns the result
            return await query.FirstOrDefaultAsync(n => n.Id == id);
        }

        // Updates an existing entity in the database asynchronously
        public async Task UpdateAsync(int id, T entity)
        {
            // Marks the entity as modified
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Modified;

            // Saves the changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
