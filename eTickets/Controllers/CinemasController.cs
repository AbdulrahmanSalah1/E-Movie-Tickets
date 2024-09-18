using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class CinemasController : Controller
    {
        AppDbContext db;
        IWebHostEnvironment _hostingEnvironment;
        private readonly IGovernsService _governsService;
        private readonly ICinemasService _cinemaService;
        private readonly IMoviesService _moviesService;
        private readonly ShoppingCart _shoppingCart;
        public CinemasController(ICinemasService service, IGovernsService governsService, IMoviesService moviesService, AppDbContext db, IWebHostEnvironment _hostingEnvironment, ShoppingCart shoppingCart)
        {
            this.db = db;
            this._hostingEnvironment = _hostingEnvironment;
            this._cinemaService = service;
            this._moviesService = moviesService;
            this._governsService = governsService;
            _shoppingCart = shoppingCart;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Retrieve all cinemas and their associated governorates
            var cinemas = await db.Cinemas
                .Include(c => c.Governorate)
                .ToListAsync();

            // Create a dictionary to hold cinema IDs and their movie counts
            var cinemaMovieCounts = new Dictionary<int, int>();

            // Populate the dictionary with movie counts for each cinema
            foreach (var cinema in cinemas)
            {
                var movieCount = db.Movies.Count(m => m.CinemaId == cinema.Id);
                cinemaMovieCounts[cinema.Id] = movieCount;
            }

            // Store the cinema movie counts in ViewData
            ViewData["MC"] = cinemaMovieCounts;
            ViewData["Cinemas"] = cinemas;
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

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var viewModel = new Filter
            {
                Movies = await _moviesService.GetAllAsync() ?? new List<Movie>(),
                Cinemas = await _cinemaService.GetAllAsync() ?? new List<Cinema>(),
                Governs = await _governsService.GetAllAsync() ?? new List<Govern>()
            };

            // إذا كانت قيمة البحث فارغة، يتم توجيه المستخدم إلى الصفحة الرئيسية
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index", "Home");
            }

            // 1. البحث عن فيلم
            if (viewModel.Movies.Any(m => m.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            {
                viewModel.Movies = viewModel.Movies
                    .Where(m => m.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // عرض صفحة الأفلام فقط
                viewModel.Cinemas = new List<Cinema>();
                viewModel.Governs = new List<Govern>();
            }

            // 2. البحث عن سينما
            if (viewModel.Cinemas.Any(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            {
                viewModel.Cinemas = viewModel.Cinemas
                    .Where(c => c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var cinemaIds = viewModel.Cinemas.Select(c => c.Id).ToList();
                viewModel.Movies = viewModel.Movies
                    .Where(m => cinemaIds.Contains(m.CinemaId))
                    .ToList();

                // عرض الأفلام المرتبطة بهذه السينما فقط
                viewModel.Governs = new List<Govern>();
            }

            // 3. البحث عن محافظة
            if (viewModel.Governs.Any(g => g.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            {
                viewModel.Governs = viewModel.Governs
                    .Where(g => g.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var governIds = viewModel.Governs.Select(g => g.Id).ToList();
                viewModel.Cinemas = viewModel.Cinemas
                    .Where(c => governIds.Contains(c.GovernId))
                    .ToList();
                // Create a dictionary to hold cinema IDs and their movie counts
                var cinemaMovieCounts = new Dictionary<int, int>();

                // Populate the dictionary with movie counts for each cinema
                foreach (var cinema in viewModel.Cinemas)
                {
                    var movieCount = db.Movies.Count(m => m.CinemaId == cinema.Id);
                    cinemaMovieCounts[cinema.Id] = movieCount;
                }

                // Store the cinema movie counts in ViewData
                ViewData["MC"] = cinemaMovieCounts;
                // عند البحث عن محافظة نعرض السينمات فقط
                viewModel.Movies = new List<Movie>();
            }
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
            // إذا لم يتم العثور على نتائج
            return View("Search",viewModel);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchString)
        {
            var model = new Filter
            {
                Movies = await _moviesService.GetAllAsync(),
                Cinemas = await _cinemaService.GetAllAsync(),
                Governs = await _governsService.GetAllAsync()
            };
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
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var cinemaDetails = await db.Cinemas.Include(c => c.Movies).FirstOrDefaultAsync(c => c.Id == id);
            if (cinemaDetails == null) return View("NotFound");
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
            return View(cinemaDetails);
        }

        //

        //Get: Cinemas/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Gov"] = db.Governs.ToList();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile Logo)
        {
            if (!ModelState.IsValid) return View(cinema);



            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Set the ProfilePictureURL property of the Actor object
                cinema.Logo = "/images/" + fileName;
            }


            var gov = db.Governs.Find(cinema.GovernId);
            var NewCinema = new Cinema
            {
                Logo = cinema.Logo,
                Name = cinema.Name,
                Description = cinema.Description,
                Governorate = gov
            };
            db.Cinemas.Add(NewCinema);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }

        //Get: Cinemas/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            ViewData["Gov"] = db.Governs.ToList();
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
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
            return View(cinemaDetails);
        }

        //Get: Cinemas/Edit/1
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            ViewData["Gov"] = db.Governs.ToList();
            if (cinemaDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(cinemaDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Cinema cinema, IFormFile Logo)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Gov"] = db.Governs.ToList(); // Ensure dropdown is populated if the model is invalid
                return View(cinema);
            }

            var existingCinema = db.Cinemas.Find(id);
            if (existingCinema == null)
            {
                return NotFound();
            }

            string oldFilePath = null;
            if (!string.IsNullOrEmpty(existingCinema.Logo))
            {
                oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", existingCinema.Logo.TrimStart('/'));
            }

            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Delete the old logo file from the server
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update the logo property of the existing cinema object
                existingCinema.Logo = "/images/" + fileName;
            }

            // Update other properties
            existingCinema.Name = cinema.Name;
            existingCinema.Description = cinema.Description;
            existingCinema.GovernId = cinema.GovernId;

            db.Cinemas.Update(existingCinema);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }

        //Get: Cinemas/Delete/1
        public IActionResult Delete(int id)
        {
            ViewData["Gov"] = db.Governs.ToList();
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            return View(cinemaDetails);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirm(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");

            db.Cinemas.Remove(cinemaDetails);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }
    }
}
