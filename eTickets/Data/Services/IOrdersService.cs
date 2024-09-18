// Import models used for orders and shopping cart items
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
// Import async functionalities
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // Interface defining operations related to order management
    public interface IOrdersService
    {
        // Asynchronously stores an order with the provided details
        Task<bool> StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, DateTime movieDate);
        // Asynchronously retrieves orders based on user ID and role
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
    }
}
