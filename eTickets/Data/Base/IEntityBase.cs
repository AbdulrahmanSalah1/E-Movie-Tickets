using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Base
{
    // Defines a base interface for entities with an ID property
    public interface IEntityBase
    {
        // Every class that implements this interface must have
        // an Id property of type int
        int Id { get; set; }
    }
}
