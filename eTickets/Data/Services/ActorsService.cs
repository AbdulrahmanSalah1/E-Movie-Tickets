using eTickets.Data.Base;
using eTickets.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // The ActorsService class is responsible for performing CRUD operations
    // on Actor entities.
    public class ActorsService : EntityBaseRepository<Actor>, IActorsService
    {
        // Constructor that takes in the application's DbContext and
        // passes it to the base class (EntityBaseRepository).
        public ActorsService(AppDbContext context) : base(context) { }
    }
}
