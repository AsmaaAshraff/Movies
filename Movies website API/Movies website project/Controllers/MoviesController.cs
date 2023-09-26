using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_website_project.Entities;
using Movies_website_project.Controllers;
using System.Numerics;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Movies_website_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly DBContext _context;

        public MoviesController(DBContext context)
        {
            _context = context;
        }
       
        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
          if (_context.Movies == null)
          {
              return NotFound();
          }
            
            return await _context.Movies.Include(m => m.Actors).Include(m=>m.Reviews).ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
          if (_context.Movies == null)
          {
              return NotFound();
          }
            var movie = await _context.Movies
                .Include(m=> m.Actors)
                .Include(m=>m.Reviews)
                .Where(m=>m.Id == id)
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // PUT: api/Movies/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<Movie>> UpdateMovie(int id,  [FromForm]Movie movie)
        {
            ActorsController actorsController = new ActorsController(_context);
            ReviewsController reviewsController = new ReviewsController(_context);
            Movie movieExists;
            List<Review> reviewList = new List<Review>();
            List<Actor> actorList = new List<Actor>();
            movieExists = _context.Movies.Where(m => m.Id == id).Include(m=>m.Actors).Include(m=>m.Reviews).FirstOrDefault();
            if (movieExists == null)
            {
                return BadRequest();
            }
            if(movie.Title != null)
            {
                var titleExists = _context.Movies.Where(m=>m.Title.ToLower() == movie.Title.ToLower()).FirstOrDefault();
                if (titleExists != null && titleExists.Id != movieExists.Id)
                {
                    return BadRequest("This movie title already exists");
                }
                movieExists.Title = movie.Title;
            }
            movieExists.ReleaseDate = movie.ReleaseDate;
            if(movie.PosterImage != null)
            {
                var uniqueFileName = movie.PosterImage.FileName;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    movie.PosterImage?.CopyTo(stream);
                }
                movieExists.PosterPath = Path.Combine("uploads", uniqueFileName);
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // If needed for case-insensitive property matching
            };
            if (!string.IsNullOrEmpty(movie.ActorsString))
            {
                actorList = JsonSerializer.Deserialize<List<Actor>>(movie.ActorsString, options);
            }
            if (actorList != null)
            {
                List<Actor> actors = new List<Actor>();
                    foreach (var actor in actorList)
                    {
                        var actorCheck = _context.actors.Where(m => m.id == actor.id).FirstOrDefault();
                        if (actorCheck == null)
                        {
                            var actorCheckTask = actorsController.PostActor(actor);
                            actorCheck = await actorCheckTask;
                             
                            //var actorExists = actors.Any(a => a.name.ToLower() == actor.name.ToLower());
                                actors.Add(actorCheck);
                            
                        }
                        else
                        {
                            actorCheck.name = actor.name;
                            bool exists = false;
                            //foreach(var a in actors)
                            //{
                            //    if(a.name.ToLower() == actor.name.ToLower()) { exists = true; break; }
                            //}
                            var actorExists = actors.Any(actor => actor.name.ToLower() == actorCheck.name.ToLower());
                            //Console.WriteLine("aaaaa"+actorExists);
                        if (actorExists == false ) 
                        {
                            actors.Add(actorCheck);
                        }
                            //var actorExists = _context.Movies.ac
                            //await actorsController.PutActor(actorCheck.Id, actorCheck);
                        }
                    }
                    movieExists.Actors = actors;
            }
            if (!string.IsNullOrEmpty(movie.ReviewsString))
            {
                reviewList = JsonSerializer.Deserialize<List<Review>>(movie.ReviewsString, options);
            }
            if (reviewList != null)
            {
                List<Review> reviews = new List<Review>();
                    foreach (var review in reviewList)
                    {
                        if(review.ReviewId!=null)
                        {
                            review.MovieId = movie.Id;
                            reviews.Add(review);
                            continue;
                        }
                        review.MovieId = movie.Id;
                        reviews.Add(review);
                        await reviewsController.PostReview(review);
                    }
                movieExists.Reviews = reviews;
            }
            await _context.SaveChangesAsync();
            return movieExists;
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public  async Task<ActionResult<Movie>> PostMovie([FromForm]Movie movie)
        {
            ActorsController actorsController = new ActorsController(_context);
            ReviewsController reviewsController = new ReviewsController(_context);
            if (_context.Movies == null)
            {
              return Problem("Entity set 'DBContext.Movies'  is null.");
            }
            //saving the poster in uploads and its path in database
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.PosterImage?.FileName;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                movie.PosterImage?.CopyTo(stream);
            }
            movie.PosterPath = Path.Combine("uploads", uniqueFileName);
            //checking for existing movie title
            var titleExist = _context.Movies.Where(m => m.Title == movie.Title).FirstOrDefault();
            if (titleExist != null)
            {
                return BadRequest("This movie title already exists");
            }
            //Cheking for the existing actors
            List<Actor> actors = new List<Actor>();
            if (movie.Actors != null)
            {
                foreach (var actor in movie.Actors)
                {
                    var actorCheck = _context.actors.Where(m => m.name.ToLower() == actor.name.ToLower()).SingleOrDefault();
                    if (actorCheck == null)
                    {
                        actorsController.PostActor(actor);
                        actors.Add(actor);
                    }
                    else
                    {
                        actors.Add(actorCheck);
                    }
                }
            }
            movie.Actors = actors;
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (_context.Movies == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
