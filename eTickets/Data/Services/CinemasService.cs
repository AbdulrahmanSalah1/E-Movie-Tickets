using eTickets.Data.Base;
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // The CinemasService class handles CRUD operations related to the Cinema
    // entity.
    public class CinemasService:EntityBaseRepository<Cinema>, ICinemasService
    {
        // Private field to hold the application's DbContext.
        private readonly AppDbContext _context;
        // Constructor that takes the AppDbContext and passes it to the base class.
        public CinemasService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        // Custom method to retrieve a Cinema by its ID.
        public async Task<Cinema> GetCinemaByIdAsync(int id)
        {
            return await _context.Cinemas.FindAsync(id);
        }
    }
}
