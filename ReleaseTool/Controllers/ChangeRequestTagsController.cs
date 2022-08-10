using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.DataAccess;
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
            if (_context.ChangeRequestTags == null)
            {
                return NotFound();
            }
            return await _context.ChangeRequestTags.ToListAsync();
        }

        // GET: api/ChangeRequestTags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChangeRequestTag>> GetChangeRequestTag(int id)
        {
            if (_context.ChangeRequestTags == null)
            {
                return NotFound();
            }
            var changeRequestTag = await _context.ChangeRequestTags.FindAsync(id);

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
            if (_context.ChangeRequestTags == null)
            {
                return Problem("Entity set 'ReleaseToolContext.ChangeRequestTag'  is null.");
            }
            if (!TagExists(changeRequestTag.TagId))
            {
                return BadRequest("Tag not found.");
            }
            if (!ChangeRequestExists(changeRequestTag.ChangeRequestId))
            {
                return BadRequest("Change request not found.");
            }

            _context.ChangeRequestTags.Add(changeRequestTag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChangeRequestTag", new { id = changeRequestTag.ChangeRequestTagId }, changeRequestTag);
        }

        // DELETE: api/ChangeRequestTags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequestTag(int id)
        {
            if (_context.ChangeRequestTags == null)
            {
                return NotFound();
            }
            var changeRequestTag = await _context.ChangeRequestTags.FindAsync(id);
            if (changeRequestTag == null)
            {
                return NotFound();
            }

            _context.ChangeRequestTags.Remove(changeRequestTag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChangeRequestTagExists(int id)
        {
            return (_context.ChangeRequestTags?.Any(e => e.ChangeRequestTagId == id)).GetValueOrDefault();
        }

        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequests?.Any(x => x.ChangeRequestId == id)).GetValueOrDefault();
        }

        private bool TagExists(int id)
        {
            return (_context.Tags?.Any(x => x.TagId == id)).GetValueOrDefault();
        }
    }
}
