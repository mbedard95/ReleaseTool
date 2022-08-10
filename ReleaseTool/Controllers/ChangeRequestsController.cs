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
    public class ChangeRequestsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;

        public ChangeRequestsController(ReleaseToolContext context)
        {
            _context = context;
        }

        // GET: api/ChangeRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChangeRequest>>> GetChangeRequest()
        {
          if (_context.ChangeRequests == null)
          {
              return NotFound();
          }
            return await _context.ChangeRequests.ToListAsync();
        }

        // GET: api/ChangeRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChangeRequest>> GetChangeRequest(int id)
        {
          if (_context.ChangeRequests == null)
          {
              return NotFound();
          }
            var changeRequest = await _context.ChangeRequests.FindAsync(id);

            if (changeRequest == null)
            {
                return NotFound();
            }

            return changeRequest;
        }

        // PUT: api/ChangeRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChangeRequest(int id, ChangeRequest changeRequest)
        {
            if (id != changeRequest.ChangeRequestId)
            {
                return BadRequest();
            }

            _context.Entry(changeRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChangeRequestExists(id))
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

        // POST: api/ChangeRequests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChangeRequest>> PostChangeRequest(ChangeRequest changeRequest)
        {
          if (_context.ChangeRequests == null)
          {
              return Problem("Entity set 'ReleaseToolContext.ChangeRequest'  is null.");
          }
            _context.ChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChangeRequest", new { id = changeRequest.ChangeRequestId }, changeRequest);
        }

        // DELETE: api/ChangeRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequest(int id)
        {
            if (_context.ChangeRequests == null)
            {
                return NotFound();
            }
            var changeRequest = await _context.ChangeRequests.FindAsync(id);
            if (changeRequest == null)
            {
                return NotFound();
            }

            _context.ChangeRequests.Remove(changeRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }
    }
}
