using eTickets.Data.Base;
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // IActorsService interface extends IEntityBaseRepository<Actor>
    // It provides the contract for service operations specific to the Actor entity.
    public interface IActorsService:IEntityBaseRepository<Actor>
    {
    }
}
