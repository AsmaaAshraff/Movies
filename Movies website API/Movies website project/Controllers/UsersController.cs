using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies_website_project.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.Pkcs;
using System.Security.Policy;
using NuGet.Common;

namespace Movies_website_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DBContext _context;

        public UsersController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                //user.Password = string() hashedPassword;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.reviews == null)
            {
                return Problem("Entity set 'DBContext.reviews'  is null.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(string email = "", string password = "")
        {
            if(email == "" || password == "") { 
                return BadRequest("Email and Password are required");
            }
            var userExists = _context.Users.Where(m => m.Email == email).FirstOrDefault();
            var hash = SHA512.Create();
            var PasswordByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = hash.ComputeHash(PasswordByteArray);
            if(email == "admin" && password == "admin")
            {
                string token = GenerateAdminToken("admin");
                return Ok(new { Token = token });
            }
            password = Convert.ToBase64String(hashedPassword);
            if (userExists == null || !string.Equals(password,userExists.Password))
            {
              return BadRequest("Invalid email or password");
            }
            else
            {
                string token = GenerateAuthenticaionToken(userExists, "user");
                return Ok(new { Token = token });
            }

        }
        [HttpPost("SignUp")]
        public async Task<ActionResult<User>> createNewUser(User user)
        {
            var userExists = _context.Users.Where(m => m.Email == user.Email).FirstOrDefault();
            var hash = SHA512.Create();
            var PasswordByteArray = Encoding.Default.GetBytes(user.Password);
            var hashedPassword = hash.ComputeHash(PasswordByteArray);
            user.Password = Convert.ToBase64String(hashedPassword);
            if (userExists != null)
            {
                return BadRequest("This email already exists");
            }
            var newUser = this.PostUser(user);
            user.Id = newUser.Id;
            string token = GenerateAuthenticaionToken(user, "User");
            return Ok(new {Token = token});
        }

        private string GenerateAuthenticaionToken(User user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5SQuDeJwDvFy98FWts15PZsi9VBNET52341234"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var Claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Role, role)
        };

            var tokenDetails = new JwtSecurityToken(claims: Claims,
                                                    expires: DateTime.Now.AddMinutes(60),
                                                    signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenDetails);
            
        }
        private string GenerateAdminToken(string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5SQuDeJwDvFy98FWts15PZsi9VBNET52341234"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var Claims = new[]
            {
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDetails = new JwtSecurityToken(claims: Claims,
                                                    expires: DateTime.Now.AddMinutes(60),
                                                    signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenDetails);
        }
            // DELETE: api/Users/5
            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
