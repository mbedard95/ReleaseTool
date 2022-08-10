using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.DataAccess;
using ReleaseTool.Models;
using ReleaseTool.Models.Enums;
using XSystem.Security.Cryptography;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        readonly IMapper _mapper;

        public UsersController(ReleaseToolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewUserDto>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var users = await _context.Users.ToListAsync();
            return users.Select(x => _mapper.Map<ViewUserDto>(x)).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewUserDto>> GetUser(int id)
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

            return _mapper.Map<ViewUserDto>(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
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
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(WriteUserDto dto)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ReleaseToolContext.Users' is null.");
            }

            if (UserNameExists(dto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var newUser = _mapper.Map<User>(dto);
            
            newUser.UserStatus = UserStatus.Active;
            newUser.Created = DateTime.Now;
            newUser.Password = HashPassword(dto.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var viewUser = _mapper.Map<ViewUserDto>(newUser);

            return CreatedAtAction("GetUser", new { id = newUser.UserId }, viewUser);
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
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private bool UserNameExists(string name)
        {
            return (_context.Users?.Any(x => x.Username == name)).GetValueOrDefault();
        }

        private string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }
    }
}
