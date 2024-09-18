using eTickets.Data.Base;
using eTickets.Data.ViewModels;
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // Interface for Cinema service operations. 
    // Extends IEntityBaseRepository<Cinema> to include standard CRUD operations for Cinema entities.
    public interface ICinemasService:IEntityBaseRepository<Cinema>
    {
        // Retrieves a specific cinema by its unique identifier.
    
        //The unique identifier of the cinema
        //A task that represents the asynchronous operation.
        //The task result contains the cinema with the specified ID.
        Task<Cinema> GetCinemaByIdAsync(int id);
    }
}
