using eTickets.Data;
using eTickets.Data.Cart;
using eTickets.Data.Static;
using eTickets.Data.ViewModels;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    public class AccountController : Controller
    {
        // Dependency injection of UserManager, SignInManager, and AppDbContext
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AccountController> _logger;
        private readonly ShoppingCart _shoppingCart;
        // Constructor to initialize the injected dependencies
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<AccountController> logger, ShoppingCart shoppingCart)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _shoppingCart = shoppingCart;

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            
            var model = new AdminDashboardVM
            {
                TotalMovies = _context.Movies.Count(),
                TotalCinemas = _context.Cinemas.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalUsers = _context.Users.Count(),
                TotalGoverns = _context.Governs.Count(),
                TotalActors = _context.Actors.Count(),
                TotalProducers = _context.Producers.Count(),
                TotalComments = _context.Comments.Count(),
                TotalRates = _context.Ratings.Count()
            };
            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        // Action to list all users in the system
        public async Task<IActionResult> Users()
        {
            // Retrieve all users
            var users = await _context.Users.ToListAsync();

            // Create a list to hold the users and their roles
            var userRoles = new List<(ApplicationUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                // Retrieve the roles for each user
                var roles = await _userManager.GetRolesAsync(user);
                // Add the user and their roles to the list
                userRoles.Add((user, roles));
            }
            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            // Pass the list of users and their roles to the view
            return View(userRoles);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                return BadRequest("User ID or Role cannot be empty.");
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Get the current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove the user from their current roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add the user to the new role
            var result = await _userManager.AddToRoleAsync(user, newRole);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction("Users");
            }
            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            // Redirect back to the Users view
            return RedirectToAction("Users");
        }
        // Action to show the login page
        public IActionResult Login() => View(new LoginVM());

        // Action to handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            // Checks if the model state is valid
            if (!ModelState.IsValid) return View(loginVM);
            // Attempts to find the user by their email address
            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            
            if(user != null)
            {
                // Checks if the provided password is correct
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        user.IsActive = true;
                        await _userManager.UpdateAsync(user);
                        if (!User.IsInRole("Admin"))
                        {
                            // Redirects to the Movies index page if login is successful
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "Account");
                        }
                    }
                }
                // If password check fails, shows an error message
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginVM);
            }
            // If user is not found, shows an error message
            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }

        // Action to show the registration page
        public IActionResult Register() => View(new RegisterVM());
        // Action to handle registration form submission
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            // Checks if the model state is valid
            if (!ModelState.IsValid) return View(registerVM);

            // Checks if the email address is already in use
            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            
            if(user != null)
            {    
                TempData["Error"] = "This email address is already in use";
                return View(registerVM);
            }
            var username = await _userManager.FindByNameAsync(registerVM.UserName);
            if (username != null)
            {
                TempData["Error"] = "This usrname is already in use";
                return View(registerVM);
            }

            // Creates a new user with the provided registration details
            var newUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.UserName
              
            };


            // Attempts to create the user in the system
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                // Adds the user to the 'User' role if creation is successful
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }
            // Returns the registration completed view
            return View("RegisterCompleted");
        }

        // Action to handle user logout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            // Signs the user out
            await _signInManager.SignOutAsync();
            // Redirects to the Movies index page
            return RedirectToAction("Index", "Home");
        }
        // Action to show the access denied page when a user tries to access a restricted area
        public IActionResult AccessDenied(string ReturnUrl)
        {
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
            // Returns the access denied view
            return View();
        }
        
        // Action to show the forgot password page
        public IActionResult ForgotPassword() => View(new ForgotPasswordVM());

        // Action to handle forgot password form submission
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            // Checks if the model state is valid
            if (!ModelState.IsValid) return View(forgotPasswordVM);

            // Finds the user by email
            var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);
            if (user == null)
            {
                // If user is not found, displays a generic message
                TempData["Error"] = "If there is an account associated with this email, you can reset your password.";
                return View(forgotPasswordVM);
            }

            // Generate the reset password token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Redirect the user to the Reset Password page with the token and email
            return RedirectToAction("ResetPassword", new { token, email = user.Email });
        }
        // Action to show the reset password page
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        // Action to handle the reset password form submission
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            // Checks if the model state is valid
            if (!ModelState.IsValid) return View(resetPasswordVM);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
            if (user == null) return View("ResetPasswordConfirmation");

            // Reset the password using the provided token
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetPasswordVM);
            }

            // Redirect to a confirmation page
            return View("ResetPasswordConfirmation");
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        //==================Profile===========================
        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ConfigProfile
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl // Adjust this if your property name is different
            };
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
            return View("Profile", model);
        }
        // GET: /Account/Edit
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ConfigProfile
            {
                FullName = user.FullName,
                Email = user.Email,
                // Add current image URL or path if needed
                ProfileImageUrl = user.ProfileImageUrl
            };
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
            return View(model);
        }

        // POST: /Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditProfile(ConfigProfile model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (model.ProfileImage != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
                var extension = Path.GetExtension(model.ProfileImage.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profile", newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = $"/images/profile/{newFileName}";
            }

            user.FullName = model.FullName;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                // Verify the current password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                    return View("Profile", model);
                }
                if (model.CurrentPassword == model.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "The new password must be different from the current password.");
                    return View("Profile", model);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Profile", model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                _logger.LogInformation($"Profile updated successfully. Profile image saved at: {_webHostEnvironment.WebRootPath}/images/profile/{user.ProfileImageUrl}");
                return RedirectToAction("Profile");
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
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
            return View("Profile", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Delete profile image from server if it exists
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Delete the user from the database
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.Email} deleted their profile successfully.");
                await _signInManager.SignOutAsync(); // Sign out the user after deletion
                return RedirectToAction("Index", "Home"); // Redirect to home page after deletion
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("Profile");
        }
        //================== Contact Us =======================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Messages()
        {
            // Fetch unread messages and include user details
            var unreadMessages = await _context.ContactUss
                .Include(m => m.User) // Include user details
                .Where(m => !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Fetch read messages and include user details
            var readMessages = await _context.ContactUss
                .Include(m => m.User)
                .Where(m => m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Mark unread messages as read
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
            // Fetch unread messages for the dashboard header
            var contactUsMessages = _context.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            // Pass both unread and read messages to the view
            var model = new StatusMessagesVM
            {
                UnreadMessages = unreadMessages,
                ReadMessages = readMessages
            };
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult ContactUs()
        {
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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ContactUs(ContactUsVM contactUsVM)
        {
            if (!ModelState.IsValid)
            {
                return View(contactUsVM);
            }

            var user = await _userManager.GetUserAsync(User);

            var contactUs = new ContactUs
            {
                UserId = user?.Id, // This might be null if the user isn't logged in
                FullName = contactUsVM.FullName,
                Email = contactUsVM.Email,
                Message = contactUsVM.Message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _shoppingCart.UserId = userId;
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            if (User.IsInRole("User"))
            {
                ViewData["CartItemsCount"] = items.Count;
            }
            _context.ContactUss.Add(contactUs);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent successfully.";
            return RedirectToAction("ContactUs");
        }
    }
}
