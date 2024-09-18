using eTickets.Models;
using eTickets.Data.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // The GovernsService class handles CRUD operations related to the Govern
    // entity.
    public class GovernsService :EntityBaseRepository<Govern>, IGovernsService
    {
        // Private field to hold the application's DbContext.
        private readonly AppDbContext _context;
        // Constructor that takes the AppDbContext and passes it to the base class.
        public GovernsService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        // Custom method to retrieve a Govern by its ID.
        public async Task<Govern> GetGovernByIdAsync(int id)
        {
            return await _context.Governs.FindAsync(id);
        }
    }
}
