using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Data.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersService _ordersService;
        private readonly AppDbContext _context;

        public OrdersController(IMoviesService moviesService, ShoppingCart shoppingCart, IOrdersService ordersService, AppDbContext context)
        {
            _moviesService = moviesService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
            _context = context;
        }
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);
            
            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            if (User.IsInRole("Admin"))
            {
                // Fetch unread messages for the dashboard header
                var contactUsMessages = _context.ContactUss.Where(m => !m.IsRead).ToList();

                // Pass messages to the view using ViewBag
                ViewBag.ContactUsMessages = contactUsMessages;
            }
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (User.IsInRole("User"))
            {
                ViewData["CartItemsCount"] = items.Count;
            }
            return View(orders);
        }
        [Authorize(Roles = "User")]
        public IActionResult ShoppingCart()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            
            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            ViewData["CartItemsCount"] = items.Count;
            return View(response);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var item = await _moviesService.GetMovieByIdAsync(id);


            if (item != null && item.AvailableTickets > 0)
            {
                //item.AvailableTickets--;
                _context.SaveChanges();
                _shoppingCart.AddItemToCart(item);
            }

            ViewData["CartItemsCount"] = _shoppingCart.GetShoppingCartItems().Count;
            return RedirectToAction(nameof(ShoppingCart));
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var item = await _moviesService.GetMovieByIdAsync(id);

            if (item != null)
            {
                //item.AvailableTickets++;
                _context.SaveChanges();
                _shoppingCart.RemoveItemFromCart(item);
            }

            ViewData["CartItemsCount"] = _shoppingCart.GetShoppingCartItems().Count;
            return RedirectToAction(nameof(ShoppingCart));
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CompleteOrder(Payment payment, DateTime expiryDate, DateTime MovieDate, int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid form data.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userEmailAddress = User.FindFirstValue(ClaimTypes.Email);

            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();

            if (MovieDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "The movie date cannot be in the past.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            if (items.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            if (string.IsNullOrWhiteSpace(payment.CardNumber) || !payment.CardNumber.All(char.IsDigit) || payment.CardNumber.Length != 16)
            {
                TempData["ErrorMessage"] = "Invalid card number format. It must be exactly 16 digits.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            if (!int.TryParse(payment.CVV, out var cvv) || payment.CVV.Length != 3)
            {
                TempData["ErrorMessage"] = "Invalid CVV format. It must be 3 digits.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            if (!DateTime.TryParseExact(payment.ExpiryDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out var parsedExpiryDate))
            {
                TempData["ErrorMessage"] = "Invalid expiry date format.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            if (parsedExpiryDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "This card is expired.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == item.Movie.Id);
                        if (movie == null || MovieDate > movie.EndDate)
                        {
                            TempData["ErrorMessage"] = "One or more movies in your cart have expired release dates and cannot be booked.";
                            return RedirectToAction(nameof(ShoppingCart));
                        }
                        if (MovieDate >= DateTime.Now && MovieDate < movie.StartDate)
                        {
                            TempData["ErrorMessage"] = "One or more movies in your cart are not available for booking.";
                            return RedirectToAction(nameof(ShoppingCart));
                        }

                        if (movie.AvailableTickets < item.Amount)
                        {
                            TempData["ErrorMessage"] = $"Not enough available tickets for movie: {movie.Name}.";
                            return RedirectToAction(nameof(ShoppingCart));
                        }

                        movie.AvailableTickets -= item.Amount;
                        _context.Movies.Update(movie);
                    }

                    await _ordersService.StoreOrderAsync(items, userId, userEmailAddress, MovieDate);
                    await _shoppingCart.ClearShoppingCartAsync();
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    ViewData["CartItemsCount"] = 0;
                    return View("CompleteOrder");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "An error occurred while processing your order. Please try again.";
                    return RedirectToAction(nameof(ShoppingCart));
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the order by ID
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Movie)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the order belongs to the current user
            if (order.UserId != userId)
            {
                return Unauthorized();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Restore the available tickets
                    foreach (var item in order.OrderItems)
                    {
                        var movie = await _context.Movies.FindAsync(item.MovieId);
                        if (movie != null)
                        {
                            movie.AvailableTickets += item.Amount;
                            _context.Movies.Update(movie);
                        }
                    }

                    // Delete the order
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
        }

    }
}
