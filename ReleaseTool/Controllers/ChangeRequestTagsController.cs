using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Models;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeRequestTagsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;

        public ChangeRequestTagsController(ReleaseToolContext context)
        {
            _context = context;
        }

        // GET: api/ChangeRequestTags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChangeRequestTag>>> GetChangeRequestTag()
        {
          if (_context.ChangeRequestTag == null)
          {
              return NotFound();
          }
            return await _context.ChangeRequestTag.ToListAsync();
        }

        // GET: api/ChangeRequestTags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChangeRequestTag>> GetChangeRequestTag(int id)
        {
          if (_context.ChangeRequestTag == null)
          {
              return NotFound();
          }
            var changeRequestTag = await _context.ChangeRequestTag.FindAsync(id);

            if (changeRequestTag == null)
            {
                return NotFound();
            }

            return changeRequestTag;
        }

        // PUT: api/ChangeRequestTags/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChangeRequestTag(int id, ChangeRequestTag changeRequestTag)
        {
            if (id != changeRequestTag.ChangeRequestTagId)
            {
                return BadRequest();
            }

            _context.Entry(changeRequestTag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChangeRequestTagExists(id))
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

        // POST: api/ChangeRequestTags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChangeRequestTag>> PostChangeRequestTag(ChangeRequestTag changeRequestTag)
        {
          if (_context.ChangeRequestTag == null)
          {
              return Problem("Entity set 'ReleaseToolContext.ChangeRequestTag'  is null.");
          }
            _context.ChangeRequestTag.Add(changeRequestTag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChangeRequestTag", new { id = changeRequestTag.ChangeRequestTagId }, changeRequestTag);
        }

        // DELETE: api/ChangeRequestTags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequestTag(int id)
        {
            if (_context.ChangeRequestTag == null)
            {
                return NotFound();
            }
            var changeRequestTag = await _context.ChangeRequestTag.FindAsync(id);
            if (changeRequestTag == null)
            {
                return NotFound();
            }

            _context.ChangeRequestTag.Remove(changeRequestTag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChangeRequestTagExists(int id)
        {
            return (_context.ChangeRequestTag?.Any(e => e.ChangeRequestTagId == id)).GetValueOrDefault();
        }
    }
}
