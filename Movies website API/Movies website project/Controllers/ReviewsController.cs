using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies_website_project.Entities;

namespace Movies_website_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly DBContext _context;

        public ReviewsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> Getreviews()
        {
          if (_context.reviews == null)
          {
              return NotFound();
          }
            return await _context.reviews.ToListAsync();
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
          if (_context.reviews == null)
          {
              return NotFound();
          }
            var review = await _context.reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // POST: api/Reviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
          if (_context.reviews == null)
          {
              return Problem("Entity set 'DBContext.reviews'  is null.");
          }
            _context.reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = review.ReviewId }, review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (_context.reviews == null)
            {
                return NotFound();
            }
            var review = await _context.reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}
