using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
namespace eTickets.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class GovernsController : Controller
    {
        AppDbContext db;
        private readonly ShoppingCart _shoppingCart;
        public GovernsController(AppDbContext db, ShoppingCart shoppingCart)

        {
            this.db = db;
            _shoppingCart = shoppingCart;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Govern> governs = db.Governs.ToList();
            if (User.IsInRole("Admin"))
            {
                // Fetch unread messages for the dashboard header
                var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

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
            return View(governs);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            // Retrieve govern details including related cinemas
            var governDetails = await db.Governs.Include(c => c.Cinemas).FirstOrDefaultAsync(c => c.Id == id);
            if (governDetails == null) return View("NotFound");

            // Create a dictionary to hold cinema IDs and their movie counts
            var cinemaMovieCounts = new Dictionary<int, int>();

            // Populate the dictionary with movie counts for each cinema
            foreach (var cinema in governDetails.Cinemas) // Fixed iteration over cinemas
            {
                var movieCount = db.Movies.Count(m => m.CinemaId == cinema.Id);
                cinemaMovieCounts[cinema.Id] = movieCount;
            }

            // Store the cinema movie counts and cinemas list in ViewData
            ViewData["MC"] = cinemaMovieCounts;
            ViewData["Cinemas"] = governDetails.Cinemas; // Fixed reference to cinemas

            if (User.IsInRole("Admin"))
            {
                // Fetch unread messages for the dashboard header
                var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

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
            return View(governDetails);
        }
        //Get: Governs/Create
        public IActionResult Create()
        {

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Govern govern)
        {
            if (!ModelState.IsValid) return View(govern);
            db.Governs.Add(govern);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            return RedirectToAction("Index");
        }

        //Get: Governs/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            if (User.IsInRole("Admin"))
            {
                // Fetch unread messages for the dashboard header
                var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

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
            return View(governDetails);
        }

        //Get: Governs/Edit/1
        public IActionResult Edit(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(governDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Govern govern)
        {
            if (id != govern.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(govern);
            }
            db.Governs.Update(govern);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }

        //Get: Governs/Delete/1
        public IActionResult Delete(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(governDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(int id)
        {
            Govern governDetails = db.Governs.Find(id);
            if (governDetails == null) return View("NotFound");
            db.Governs.Remove(governDetails);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            return RedirectToAction("Index");
        }
    }
}
