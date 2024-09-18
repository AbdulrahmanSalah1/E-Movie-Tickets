using eTickets.Data.Base;
using eTickets.Models;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // Interface for Govern service operations. 
    // Extends IEntityBaseRepository<Govern> to include standard CRUD operations for Govern entities.
    public interface IGovernsService:IEntityBaseRepository<Govern>
    {
        // Retrieves a specific govern entity by its unique identifier.
        // The unique identifier of the govern entity.
        // A task that represents the asynchronous operation.
        // The task result contains the govern entity with the specified ID.
        Task<Govern> GetGovernByIdAsync(int id);
    }
}
