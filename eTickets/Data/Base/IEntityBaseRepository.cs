using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eTickets.Data.Base
{
    // Generic repository interface for entity classes
    public interface IEntityBaseRepository<T> where T: class, IEntityBase, new()
    {
        // Retrieves all entities asynchronously
        Task<IEnumerable<T>> GetAllAsync();
        // Retrieves all entities with related data included, using expression trees to specify
        // which properties to include
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        // Retrieves a single entity by its ID asynchronously
        Task<T> GetByIdAsync(int id);
        // Retrieves a single entity by its ID with related data included, using expression trees
        // to specify which properties to include
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        // Adds a new entity asynchronously
        Task AddAsync(T entity);
        // Updates an existing entity asynchronously
        Task UpdateAsync(int id, T entity);
        // Deletes an entity by its ID asynchronously
        Task DeleteAsync(int id);
    }
}
