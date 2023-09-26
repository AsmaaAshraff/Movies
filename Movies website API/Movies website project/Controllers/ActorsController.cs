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
    public class ActorsController : ControllerBase
    {
        private readonly DBContext _context;

        public ActorsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Getactors()
        {
            if (_context.actors == null)
            {
                return NotFound();
            }
            return await _context.actors.ToListAsync();
        }

        // GET: api/Actors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(int id)
        {
            if (_context.actors == null)
            {
                return NotFound();
            }
            var actor = await _context.actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return actor;
        }

        // PUT: api/Actors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Actor>> PutActor(int id, Actor actor)
        {

            if (id != actor.id)
            {
                return BadRequest();
            }

            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return actor;
        }

        // POST: api/Actors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<Actor> PostActor(Actor actor)
        {
            
            var actorExists = _context.actors.Where(m => m.name == actor.name).FirstOrDefault();
            if (actorExists != null)
            {
                return actorExists;
            }
            _context.actors.AddAsync(actor);
            await _context.SaveChangesAsync();
            var created = CreatedAtAction("GetActor", new { id = actor.id }, actor);
            return actor;
        }
       
        // DELETE: api/Actors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            if (_context.actors == null)
            {
                return NotFound();
            }
            var actor = await _context.actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.actors.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActorExists(int id)
        {
            return (_context.actors?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
