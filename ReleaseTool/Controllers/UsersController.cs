using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Users;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;
        private readonly IUsersProvider _usersProvider;

        public UsersController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator, IUsersProvider usersProvider)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _usersProvider = usersProvider;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetUsers(bool includeInactive)
        {
            var users = await _context.Users.ToListAsync();

            return includeInactive == true ?
                users.Select(x => _mapper.Map<ReadUserDto>(x)).ToList()
                : users.Where(x => x.UserStatus != UserStatus.Inactive)
                .Select(x => _usersProvider.ConvertToView(x)).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadUserDto>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _usersProvider.ConvertToView(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, WriteUserDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var validationResult = _validator.IsValidUser(dto, id);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            user = _mapper.Map(dto, user);
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_usersProvider.UserExists(id))
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
            var id = Guid.NewGuid();
            var validationResult = _validator.IsValidUser(dto, id);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var newUser = _usersProvider.GetNewUser(dto, id);

            _context.Users.Add(newUser);
            _usersProvider.SaveUserGroupMaps(dto, newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = newUser.UserId }, _mapper.Map<ReadUserDto>(newUser));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.UserStatus == UserStatus.Inactive)
            {
                return NotFound();
            }

            user.UserStatus = UserStatus.Inactive;
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_usersProvider.UserExists(id))
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
    }
}
