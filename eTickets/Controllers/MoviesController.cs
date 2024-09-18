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
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMoviesService _service;
        private readonly IWebHostEnvironment _environment;
        private readonly ShoppingCart _shoppingCart;
        public MoviesController(IMoviesService service, IWebHostEnvironment environment, AppDbContext context, ShoppingCart shoppingCart)
        {
            _service = service;
            _environment = environment;
            _context = context;
            _shoppingCart = shoppingCart;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);
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
            return View(allMovies);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString, MovieCategory? movieCategory)
        {
            var allMovies = await _service.GetAllAsync(n => n.Cinema);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResultNew = allMovies.Where(n => string.Equals(n.Name, searchString, StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)
                    || (n.MovieCategory.ToString() != null && string.Equals(n.MovieCategory.ToString(), searchString, StringComparison.CurrentCultureIgnoreCase))).ToList();
                
                if (User.IsInRole("User"))
                {
                    ViewData["CartItemsCount"] = items.Count;
                }
                return View("Index", filteredResultNew);
            }

            if (movieCategory.HasValue)
            {
                var filteredResultByCategory = allMovies.Where(n => n.MovieCategory == movieCategory).ToList();
                
                if (User.IsInRole("User"))
                {
                    ViewData["CartItemsCount"] = items.Count;
                }
                return View("Index", filteredResultByCategory);
            }
            _shoppingCart.ShoppingCartItems = items;
            if (User.IsInRole("User"))
            {
                ViewData["CartItemsCount"] = items.Count;
            }
            return View("Index", allMovies);
        }


        //GET: Movies/Details/1
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            if (movieDetail == null)
            {
                // Log and handle the case where movieDetail is null
                return View("NotFound");
            }
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
            return View(movieDetail);
        }
        // MoviesController.cs

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddRating(int movieId, int ratingValue)
        {
            if (movieId <= 0 || ratingValue < 1 || ratingValue > 5)
            {
                return BadRequest("Invalid movie ID or rating value.");
            }

            try
            {
                var movie = await _context.Movies.Include(m => m.Ratings).FirstOrDefaultAsync(m => m.Id == movieId);

                if (movie == null)
                {
                    return NotFound("Movie not found.");
                }

                if (movie.Ratings == null)
                {
                    movie.Ratings = new List<Rating>();
                }

                var rating = new Rating
                {
                    Value = ratingValue,
                    UserName = User.Identity.Name
                };

                movie.Ratings.Add(rating);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = movieId });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddComment(int movieId, string commentText)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.AddCommentAsync(movieId, commentText, User.Identity.Name);
                return RedirectToAction("Details", new { id = movieId });
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate view or error message
                return StatusCode(500, "Internal server error");
            }
        }


        //GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie, IFormFile poster_img)
        {
            if (!ModelState.IsValid)
            {
                // Load dropdown values for the view if model state is invalid
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                // Handle file upload
                if (poster_img != null && poster_img.Length > 0)
                {
                    var path = Path.Combine(_environment.WebRootPath, "images");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var filePath = Path.Combine(path, poster_img.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await poster_img.CopyToAsync(stream);
                    }

                    movie.ImageURL = "/images/" + poster_img.FileName;
                }
                else
                {
                    movie.ImageURL = "default.jpeg"; // Default image path if no image is uploaded
                }

                try
                {
                    await _service.AddNewMovieAsync(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.exc = ex.Message;
                }

                return View(movie);
            }

            // Handle valid state
            if (poster_img != null && poster_img.Length > 0)
            {
                var path = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path, poster_img.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await poster_img.CopyToAsync(stream);
                }

                movie.ImageURL = "/images/" + poster_img.FileName;
            }
            else
            {
                movie.ImageURL = "default.jpeg"; // Default image path if no image is uploaded
            }

            await _service.AddNewMovieAsync(movie);
            await _context.SaveChangesAsync();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;


            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        //GET: Movies/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var movieDetails = await _service.GetMovieByIdAsync(id);
            if (movieDetails == null) return View("NotFound");

            var response = new NewMovieVM()
            {
                Id = movieDetails.Id,
                Name = movieDetails.Name,
                Description = movieDetails.Description,
                Price = movieDetails.Price,
                StartDate = movieDetails.StartDate,
                EndDate = movieDetails.EndDate,
                ShowTime = movieDetails.ShowTime,
                ImageURL = movieDetails.ImageURL,
                MovieCategory = movieDetails.MovieCategory,
                AvailableTickets = movieDetails.AvailableTickets,
                CinemaId = movieDetails.CinemaId,
                ProducerId = movieDetails.ProducerId,
                ActorIds = movieDetails.Actors_Movies.Select(n => n.ActorId).ToList(),
            };

            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();
            ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(response);
        }
        ////////////////////////////////////////////////
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie, IFormFile poster_img)
        {
            if (id != movie.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.Cinemas = new SelectList(movieDropdownsData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownsData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownsData.Actors, "Id", "FullName");

                string path = Path.Combine(_environment.WebRootPath, "poster_img");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (poster_img != null)
                {


                    path = Path.Combine(path, poster_img.FileName);

                    // for exmple : /Img/Photoname.png
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await poster_img.CopyToAsync(stream);

                        movie.ImageURL = "\\poster_img\\" + poster_img.FileName;
                        //movie.ImageURL = "~\\wwwroot\\poster_img\\" + poster_img.FileName;
                    }
                }
                else
                {
                    movie.ImageURL = "default.jpeg"; // to save the default image path in database.
                }
                try
                {
                    await _service.UpdateMovieAsync(movie);
                    //  await _service.AddNewMovieAsync(movie);

                    _context.SaveChanges();

                    //_context.AddRangeAsync(movie, poster_img);
                    // _context.SaveChanges();


                    return RedirectToAction("Index");
                }
                catch (Exception ex) { ViewBag.exc = ex.Message; }
                return View(movie);
            }

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;


            return RedirectToAction(nameof(Index));
        }
        //Get: Movies/Delete/1
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            Movie movieDetails = _context.Movies.Find(id);
            if (movieDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(movieDetails);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirm(int id)
        {
            Movie movieDetails = _context.Movies.Find(id);
            if (movieDetails == null) return View("NotFound");

            _context.Movies.Remove(movieDetails);
            _context.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }
    }
}
