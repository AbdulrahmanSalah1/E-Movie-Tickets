using eTickets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Cart
{
    public class ShoppingCart
    {
        // The database context to interact with the database
        public AppDbContext _context { get; set; }
        // Unique identifier for the shopping cart
        public string ShoppingCartId { get; set; }
        // User's unique identifier
        public string UserId { get; set; }
        // List of items in the shopping cart
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        // Constructor that initializes the database context
        public ShoppingCart(AppDbContext context)
        {
            _context = context;
        }
        // Retrieves the shopping cart for the current user/session
        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            // Retrieve session from the HTTP context
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDbContext>();
            // Retrieve or generate a unique cart ID for the session
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
            // Return a new instance of ShoppingCart with the generated cart ID
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }
        // Adds a movie to the shopping cart
        public void AddItemToCart(Movie movie)
        {
            // Find the item in the cart or create a new one if it doesn't exist
            var shoppingCartItem = 
                _context.ShoppingCartItems.FirstOrDefault(n => n.Movie.Id == movie.Id && n.ShoppingCartId == UserId);

            if(shoppingCartItem == null)
            {
                // Create a new cart item if not found
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = UserId,
                    Movie = movie,
                    Amount = 1
                };

                _context.ShoppingCartItems.Add(shoppingCartItem);
            } else
            {
                // If the item exists, increment its amount
                shoppingCartItem.Amount++;
            }
            // Save changes to the database
            _context.SaveChanges();
        }
        // Removes a movie from the shopping cart
        public void RemoveItemFromCart(Movie movie)
        {
            // Find the item in the cart
            var shoppingCartItem = 
                _context.ShoppingCartItems.FirstOrDefault(n => n.Movie.Id == movie.Id && n.ShoppingCartId == UserId);

            if (shoppingCartItem != null)
            {
                // If more than one, decrement the amount; otherwise, remove the item
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                } else
                {
                    _context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            // Save changes to the database
            _context.SaveChanges();
        }
        // Retrieves all items in the shopping cart
        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            // If ShoppingCartItems is null, fetch items from the database
            return ShoppingCartItems ?? (ShoppingCartItems = 
                _context.ShoppingCartItems.Where(n => n.ShoppingCartId == UserId).Include(n => n.Movie).ToList());
        }
        // Calculates the total cost of items in the shopping cart
        public double GetShoppingCartTotal() =>  
            _context.ShoppingCartItems.Where(n => n.ShoppingCartId == UserId).Select(n => n.Movie.Price * n.Amount).Sum();
        // Clears the shopping cart asynchronously
        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == UserId).ToListAsync();
            // Remove all items from the cart
            _context.ShoppingCartItems.RemoveRange(items);
            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
