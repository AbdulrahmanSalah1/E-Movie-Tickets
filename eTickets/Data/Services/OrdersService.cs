using eTickets.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        // Constructor that accepts AppDbContext, enabling
        // access to the database context.
        public OrdersService(AppDbContext context)
        {
            _context = context;
        }
        // Method to retrieve orders based on user ID and role.
        public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            // Retrieve all orders including their related items, movies, cinemas, and users.
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Movie).ThenInclude(n => n.Cinema).Include(n => n.User).ToListAsync();
            // If the user is not an admin, filter orders by user ID.
            if (userRole != "Admin")
            {
                orders = orders.Where(n => n.UserId == userId).ToList();
            }

            return orders;
        }
        // Method to store a new order along with its items.
        public async Task<bool> StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, DateTime movieDate)
        {
            // Validate movie dates for each item in the cart.
            foreach (var item in items)
            {
                var movie = await _context.Movies.FindAsync(item.Movie.Id);
                if (movieDate < DateTime.Now)
                {
                    // If the movie's release date has expired, refuse to book the ticket.
                    return false;
                }
            }
            // Create a new order and populate its properties.
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress,
                OrderDate = DateTime.Now,
                MovieDate = movieDate
            };
            // Add the order to the database.
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            // Add each item from the shopping cart as an order item.
            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    MovieId = item.Movie.Id,
                    OrderId = order.Id,
                    Price = item.Movie.Price
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            // Save changes to the database.
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
