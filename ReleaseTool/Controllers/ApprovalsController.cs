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
    public class ApprovalsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;

        public ApprovalsController(ReleaseToolContext context)
        {
            _context = context;
        }

        // GET: api/Approvals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Approval>>> GetApproval()
        {
          if (_context.Approval == null)
          {
              return NotFound();
          }
            return await _context.Approval.ToListAsync();
        }

        // GET: api/Approvals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Approval>> GetApproval(int id)
        {
          if (_context.Approval == null)
          {
              return NotFound();
          }
            var approval = await _context.Approval.FindAsync(id);

            if (approval == null)
            {
                return NotFound();
            }

            return approval;
        }

        // PUT: api/Approvals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApproval(int id, Approval approval)
        {
            if (id != approval.ApprovalId)
            {
                return BadRequest();
            }

            _context.Entry(approval).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalExists(id))
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

        // POST: api/Approvals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Approval>> PostApproval(Approval approval)
        {
          if (_context.Approval == null)
          {
              return Problem("Entity set 'ReleaseToolContext.Approval'  is null.");
          }
            _context.Approval.Add(approval);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApproval", new { id = approval.ApprovalId }, approval);
        }

        // DELETE: api/Approvals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApproval(int id)
        {
            if (_context.Approval == null)
            {
                return NotFound();
            }
            var approval = await _context.Approval.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }

            _context.Approval.Remove(approval);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApprovalExists(int id)
        {
            return (_context.Approval?.Any(e => e.ApprovalId == id)).GetValueOrDefault();
        }
    }
}
