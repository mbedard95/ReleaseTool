using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.Dtos;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public TagsController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags(bool includeInactive)
        {
            return includeInactive == true ? await _context.Tags.OrderBy(x => x.Name).ToListAsync()
                : await _context.Tags.Where(x => x.TagStatus != TagStatus.Inactive).OrderBy(x => x.Name).ToListAsync();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // PUT: api/Tags/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(Guid id, WriteTagDto dto)
        {
            var tag = _context.Tags.FirstOrDefault(x => x.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            tag = _mapper.Map(dto, tag);
            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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
        public async Task<ActionResult<Tag>> PostTag(WriteTagDto dto)
        {
            var validationResult = _validator.IsValidTag(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var tag = _mapper.Map<Tag>(dto);
            tag.Created = DateTime.Now;
            tag.TagStatus = TagStatus.Active;

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, tag);
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null || tag.TagStatus == TagStatus.Inactive)
            {
                return NotFound();
            }

            tag.TagStatus = TagStatus.Inactive;
            _context.Entry(tag).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        private bool TagExists(Guid id)
        {
            return (_context.Tags?.Any(e => e.TagId == id)).GetValueOrDefault();
        }        
    }
}
