using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<Tag>>> GetTag()
        {
            if (_context.Tags == null)
            {
                return NotFound();
            }
            return await _context.Tags.ToListAsync();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            if (_context.Tags == null)
            {
                return NotFound();
            }
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
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.TagId)
            {
                return BadRequest();
            }

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

        // POST: api/Tags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(WriteTagDto dto)
        {
            if (_context.Tags == null)
            {
                return Problem("Entity set 'ReleaseToolContext.Tag'  is null.");
            }
            
            var validationResult = _validator.IsValidTag(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var tag = _mapper.Map<Tag>(dto);
            tag.Created = DateTime.Now;

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTag", new { id = tag.TagId }, tag);
        }

        // DELETE: api/Tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            if (_context.Tags == null)
            {
                return NotFound();
            }
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TagExists(int id)
        {
            return (_context.Tags?.Any(e => e.TagId == id)).GetValueOrDefault();
        }        
    }
}
