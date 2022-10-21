using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using XSystem.Security.Cryptography;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public UsersController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetUsers(bool includeInactive)
        {
            var users = await _context.Users.ToListAsync();

            return includeInactive == true ?
                users.Select(x => _mapper.Map<ReadUserDto>(x)).ToList()
                : users.Where(x => x.UserStatus != UserStatus.Inactive)
                .Select(x => ConvertToView(x)).ToList();
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

            return ConvertToView(user);
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
            var id = Guid.NewGuid();
            var validationResult = _validator.IsValidUser(dto, id);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var newUser = GetNewUser(dto, id);

            _context.Users.Add(newUser);
            SaveUserGroupMaps(dto, newUser);
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

        private User GetNewUser(WriteUserDto dto, Guid id)
        {
            var newUser = _mapper.Map<User>(dto);
            newUser.Password = HashPassword(dto.Password);

            newUser.UserId = id;
            newUser.UserStatus = UserStatus.Active;
            newUser.Created = DateTime.Now;

            return newUser;
        }

        private bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private static string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }

        private List<string> GetGroupNames(Guid userId)
        {
            List<string> groupNames = new();

            var groupMaps = _context.UserGroups.Where(x => x.UserId == userId).ToList();
            foreach (var group in groupMaps)
            {
                var groupObject = _context.Groups.FirstOrDefault(x => x.GroupId == group.GroupId);
                if (groupObject != null)
                {
                    groupNames.Add(groupObject.GroupName);
                }
            }
            return groupNames;
        }

        private void SaveUserGroupMaps(WriteUserDto dto, User user)
        {
            _context.RemoveRange(_context.UserGroups.Where(x => x.UserId == user.UserId));
            foreach (var groupName in dto.Groups)
            {
                var group = _context.Groups.FirstOrDefault(x => x.GroupName == groupName && x.GroupStatus == GroupStatus.Active);
                if (group != null)
                {
                    _context.UserGroups.Add(new UserGroup
                    {
                        UserId = user.UserId,
                        GroupId = group.GroupId
                    });
                }
            }
        }

        private ReadUserDto ConvertToView(User user)
        {
            var result = _mapper.Map<ReadUserDto>(user);
            result.Groups = GetGroupNames(result.UserId);
            return result;
        }
    }
}
