// Import base repository functionalities
using eTickets.Data.Base;
// Import view models used for data transfer
using eTickets.Data.ViewModels;
// Import movie model
using eTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
// Import async functionalities
using System.Threading.Tasks;

namespace eTickets.Data.Services
{
    // Interface defining operations related to movie data
    public interface IMoviesService:IEntityBaseRepository<Movie>
    {
        Task AddCommentAsync(int movieId, string commentText, string userName);
        Task AddRatingAsync(int movieId, int ratingValue, string userName);
        // Asynchronously retrieves a movie by its unique identifier
        Task<Movie> GetMovieByIdAsync(int id);
        // Asynchronously retrieves dropdown values for creating or updating
        // movies
        Task<NewMovieDropdownsVM> GetNewMovieDropdownsValues();
        // Asynchronously adds a new movie with the provided data
        Task AddNewMovieAsync(NewMovieVM data);
        // Asynchronously updates an existing movie with the provided data
        Task UpdateMovieAsync(NewMovieVM data);
        // This method signature appears to be a duplicate of the previous
        // AddNewMovieAsync method without parameters
        // It is likely intended to be removed or corrected
        Task AddNewMovieAsync();
    }
}
