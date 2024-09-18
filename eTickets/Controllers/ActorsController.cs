﻿using eTickets.Data;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ActorsController : Controller
    {
        AppDbContext db;
        IWebHostEnvironment _hostingEnvironment;
        private readonly ShoppingCart _shoppingCart;
        public ActorsController(AppDbContext db, IWebHostEnvironment _hostingEnvironment, ShoppingCart shoppingCart)

        {
            this.db = db;
            this._hostingEnvironment = _hostingEnvironment;
            _shoppingCart = shoppingCart;
        }

        public IActionResult Index()
        {
            List<Actor> actors = db.Actors.ToList();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(actors);
        }
        //Get: Actors/Create
        public IActionResult Create()
        {
            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile ProfilePictureURL)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            if (ProfilePictureURL != null && ProfilePictureURL.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(ProfilePictureURL.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePictureURL.CopyTo(fileStream);
                }

                // Set the ProfilePictureURL property of the Actor object
                actor.ProfilePictureURL = "/images/" + fileName;
            }

            db.Actors.Add(actor);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }
        //Get: Actors/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Actor actorDetails = db.Actors.Find(id);

            if (actorDetails == null) return View("NotFound");

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
            return View(actorDetails);
        }

        //Get: Actors/Edit/1
        public IActionResult Edit(int id)
        {


            Actor actorDetails = db.Actors.Find(id);
            if (actorDetails == null) return View("NotFound");

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(actorDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Actor actor, IFormFile ProfilePictureURL)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            string oldFilePath = null;
            if (!string.IsNullOrEmpty(actor.ProfilePictureURL))
            {
                oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", actor.ProfilePictureURL.TrimStart('/'));
            }

            if (ProfilePictureURL != null && ProfilePictureURL.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(ProfilePictureURL.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePictureURL.CopyTo(fileStream);
                }

                // Delete the old profile picture file from the server
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Set the ProfilePictureURL property of the Actor object
                actor.ProfilePictureURL = "/images/" + fileName;
            }

            db.Actors.Update(actor);
            db.SaveChanges();


            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;


            return RedirectToAction("Index");
        }


        //Get: Actors/Delete/1
        public IActionResult Delete(int id)
        {
            Actor actorDetails = db.Actors.Find(id);
            if (actorDetails == null) return View("NotFound");


            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return View(actorDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Actor actorDetails = db.Actors.Find(id);
            if (actorDetails == null) return View("NotFound");

            db.Actors.Remove(actorDetails);
            db.SaveChanges();

            // Fetch unread messages for the dashboard header
            var contactUsMessages = db.ContactUss.Include(m => m.User).Where(m => !m.IsRead).ToList();

            // Pass messages to the view using ViewBag
            ViewBag.ContactUsMessages = contactUsMessages;

            return RedirectToAction("Index");
        }
    }
}




