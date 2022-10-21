using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;

        public ApprovalsController(ReleaseToolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Approvals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Approval>>> GetApprovals(bool includeInactive)
        {
            if (_context.Approvals == null)
            {
                return Problem("Entity set is null.");
            }
            return includeInactive == true ? await _context.Approvals.ToListAsync()
                : await _context.Approvals.Where(x => x.ApprovalStatus != ApprovalStatus.Removed).ToListAsync();
        }

        // GET: api/Approvals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Approval>> GetApproval(int id)
        {
            if (_context.Approvals == null)
            {
                return Problem("Entity set is null.");
            }
            var approval = await _context.Approvals.FindAsync(id);

            if (approval == null)
            {
                return NotFound();
            }

            return approval;
        }

        // PUT: api/Approvals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApproval(Guid id, UpdateApprovalDto dto)
        {
            if (_context.Approvals == null)
            {
                return Problem("Entity set is null.");
            }
            var approval = _context.Approvals.FirstOrDefault(x => x.ApprovalId == id);
            if (approval == null)
            {
                return NotFound();
            }

            approval = _mapper.Map(dto, approval);
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

        // DELETE: api/Approvals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApproval(Guid id)
        {
            if (_context.Approvals == null)
            {
                return Problem("Entity set is null.");
            }
            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }

            _context.Approvals.Remove(approval);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApprovalExists(Guid id)
        {
            return (_context.Approvals?.Any(e => e.ApprovalId == id)).GetValueOrDefault();
        }

        
    }
}
