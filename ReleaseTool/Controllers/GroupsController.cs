using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Groups.Models.Dtos;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public GroupsController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups(bool includeInactive)
        {
            return includeInactive == true ? await _context.Groups.ToListAsync()
                : await _context.Groups.Where(x => x.GroupStatus != GroupStatus.Inactive).ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroups(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(Guid id, WriteGroupDto dto)
        {
            var group = _context.Groups.FirstOrDefault(x => x.GroupId == id);
            if (group == null)
            {
                return NotFound();
            }

            group = _mapper.Map(dto, group);
            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(WriteGroupDto dto)
        {
            var validationResult = _validator.IsValidGroup(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var group = _mapper.Map<Group>(dto);
            group.Created = DateTime.Now;
            group.GroupStatus = GroupStatus.Active;

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroups", new { id = group.GroupId }, group);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || group.GroupStatus == GroupStatus.Inactive)
            {
                return NotFound();
            }

            group.GroupStatus = GroupStatus.Inactive;
            _context.Entry(group).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        private bool GroupExists(Guid id)
        {
            return (_context.Groups?.Any(e => e.GroupId == id)).GetValueOrDefault();
        }
    }
}
