using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace eTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ShoppingCart _shoppingCart;
        public HomeController(AppDbContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }
        public IActionResult Index()
        {
            var cinemas = _context.Cinemas.Include(c => c.Movies).ToList();
            if (User.IsInRole("Admin"))
            {
                // Fetch unread messages for the dashboard header
                var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

                // Pass messages to the view using ViewBag
                ViewBag.ContactUsMessages = contactUsMessages;
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (User.IsInRole("User"))
            {
                ViewData["CartItemsCount"] = items.Count;
            }
            return View(cinemas);
        }
    }
}
