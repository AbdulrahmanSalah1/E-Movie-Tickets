// Includes the models (ApplicationUser, Actor, Movie, etc.)
using eTickets.Models;
// Provides identity-related context functionality
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// Provides Entity Framework Core functionality
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data
{
    // DbContext for the application, includes identity and application-specific entities
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        // Constructor that accepts DbContext options and passes them to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // DbSet for managing ApplicationUser entities in the database
        public DbSet<ApplicationUser>Accounts { get; set; }
        public DbSet<ContactUs> ContactUss { get; set; }
        // Override OnModelCreating to configure entity relationships and keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key configuration for the Actor_Movie join table
            modelBuilder.Entity<Actor_Movie>().HasKey(am => new
            {
                am.ActorId,
                am.MovieId
            });
            // Configure the one-to-many relationship between Movie and Actor_Movie
            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Movie).WithMany(am => am.Actors_Movies).HasForeignKey(m => m.MovieId);
            // Configure the one-to-many relationship between Actor and Actor_Movie
            modelBuilder.Entity<Actor_Movie>().HasOne(m => m.Actor).WithMany(am => am.Actors_Movies).HasForeignKey(m => m.ActorId);
            // Configure the relationship if needed
            modelBuilder.Entity<ContactUs>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ContactUss)
                .HasForeignKey(cm => cm.UserId);
            // Call the base method to ensure that identity-related configuration is applied
            base.OnModelCreating(modelBuilder);
        }
       
        // DbSet properties for various entities in the application
        public DbSet<Actor> Actors { get; set; } // Table for Actor entities
        public DbSet<Movie> Movies { get; set; } // Table for Movie entities
        public DbSet<Actor_Movie> Actors_Movies { get; set; } // Table for Actor_Movie join entities
        public DbSet<Govern> Governs { get; set; } // Table for Govern entities
        public DbSet<Cinema> Cinemas { get; set; } // Table for Cinema entities
        public DbSet<Producer> Producers { get; set; } // Table for Producer entities
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Rating> Ratings { get; set; }
        //Orders related tables
        public DbSet<Order> Orders { get; set; } // Table for Order entities
        public DbSet<OrderItem> OrderItems { get; set; } // Table for OrderItem entities
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } // Table for ShoppingCartItem entities
    }
}
