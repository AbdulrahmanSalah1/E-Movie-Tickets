﻿using eTickets.Data.Base;
using eTickets.Data.ViewModels;
using eTickets.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // MoviesService class handles CRUD operations related to movies.
    public class MoviesService : EntityBaseRepository<Movie>, IMoviesService
    {
        private readonly AppDbContext _context;

        public MoviesService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies.Include(m => m.Cinema).ToListAsync();
        }

        public async Task AddNewMovieAsync(NewMovieVM data)
        {
            var newMovie = new Movie
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                ImageURL = data.ImageURL,
                CinemaId = data.CinemaId,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                ShowTime = data.ShowTime,
                AvailableTickets = data.AvailableTickets,
                MovieCategory = data.MovieCategory,
                ProducerId = data.ProducerId
            };

            await _context.Movies.AddAsync(newMovie);
            await _context.SaveChangesAsync();

            foreach (var actorId in data.ActorIds)
            {
                var newActorMovie = new Actor_Movie
                {
                    MovieId = newMovie.Id,
                    ActorId = actorId
                };
                await _context.Actors_Movies.AddAsync(newActorMovie);
            }
            await _context.SaveChangesAsync();
        }

        public Task AddNewMovieAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies
                    .Include(c => c.Cinema)
                    .Include(p => p.Producer)
                    .Include(am => am.Actors_Movies).ThenInclude(a => a.Actor)
                    .Include(m => m.Comments)
                    .Include(m => m.Ratings) // Ensure Ratings are included
                    .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues()
        {
            return new NewMovieDropdownsVM
            {
                Actors = await _context.Actors.OrderBy(n => n.FullName).ToListAsync(),
                Cinemas = await _context.Cinemas.OrderBy(n => n.Name).ToListAsync(),
                Producers = await _context.Producers.OrderBy(n => n.FullName).ToListAsync()
            };
        }

        public async Task AddCommentAsync(int movieId, string commentText, string userName)
        {
            var movie = await _context.Movies
                .Include(m => m.Comments) // Ensure Comments are included
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            if (string.IsNullOrEmpty(commentText))
            {
                throw new ArgumentException("Comment text cannot be empty.");
            }

            // Initialize Comments collection if it's null
            if (movie.Comments == null)
            {
                movie.Comments = new List<Comment>();
            }

            var comment = new Comment
            {
                Text = commentText,
                Date = DateTime.Now,
                UserName = userName
            };

            movie.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }


        public async Task AddRatingAsync(int movieId, int ratingValue, string userName)
        {
            if (ratingValue < 1 || ratingValue > 5)
            {
                throw new ArgumentException("Rating value must be between 1 and 5.");
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                throw new ArgumentException("Movie not found.");
            }

            // Initialize Ratings collection if it's null
            if (movie.Ratings == null)
            {
                movie.Ratings = new List<Rating>();
            }

            var rating = new Rating
            {
                Value = ratingValue,
                UserName = userName
            };

            movie.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateMovieAsync(NewMovieVM data)
        {
            var dbMovie = await _context.Movies.FirstOrDefaultAsync(n => n.Id == data.Id);

            if (dbMovie != null)
            {
                dbMovie.Name = data.Name;
                dbMovie.Description = data.Description;
                dbMovie.Price = data.Price;
                dbMovie.ImageURL = data.ImageURL;
                dbMovie.CinemaId = data.CinemaId;
                dbMovie.StartDate = data.StartDate;
                dbMovie.EndDate = data.EndDate;
                dbMovie.ShowTime = data.ShowTime;
                dbMovie.MovieCategory = data.MovieCategory;
                dbMovie.AvailableTickets = data.AvailableTickets;
                dbMovie.ProducerId = data.ProducerId;

                await _context.SaveChangesAsync();

                var existingActorsDb = _context.Actors_Movies.Where(n => n.MovieId == data.Id).ToList();
                _context.Actors_Movies.RemoveRange(existingActorsDb);
                await _context.SaveChangesAsync();

                foreach (var actorId in data.ActorIds)
                {
                    var newActorMovie = new Actor_Movie
                    {
                        MovieId = data.Id,
                        ActorId = actorId
                    };
                    await _context.Actors_Movies.AddAsync(newActorMovie);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
