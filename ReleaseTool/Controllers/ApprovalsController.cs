﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Models;

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
        public async Task<ActionResult<IEnumerable<Approval>>> GetApprovals(Guid changeRequestId)
        {
            return changeRequestId == Guid.Empty ? await _context.Approvals.ToListAsync()
                : await _context.Approvals
                .Where(x => x.ApprovalStatus != ApprovalStatus.Removed
                && x.ChangeRequestId == changeRequestId)
                .ToListAsync();
        }

        // GET: api/Approvals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Approval>> GetApproval(int id)
        {
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
            var approval = _context.Approvals.FirstOrDefault(x => x.ApprovalId == id && x.UserId == dto.UserId);
            if (approval == null)
            {
                return NotFound();
            }

            if (approval.ApprovalStatus != ApprovalStatus.Approved
                && dto.ApprovalStatus == ApprovalStatus.Approved)
            {
                approval.ApprovedDate = DateTime.UtcNow;
            }
            if (approval.ApprovalStatus == ApprovalStatus.Approved
                && dto.ApprovalStatus != ApprovalStatus.Approved)
            {
                approval.ApprovedDate = DateTime.MaxValue;
            }
            approval = _mapper.Map(dto, approval);
            _context.Entry(approval).State = EntityState.Modified;
            UpdateChangeRequest(approval.ChangeRequestId, approval.ApprovalId, dto);

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

        private void UpdateChangeRequest(Guid changeRequestId, Guid approvalId, UpdateApprovalDto dto)
        {
            var changeRequest = _context.ChangeRequests.Find(changeRequestId);
            if (changeRequest != null)
            {
                var approvalsList = _context.Approvals
                    .Where(x => x.ChangeRequestId == changeRequestId && x.ApprovalId != approvalId)
                    .ToList();
                var approved = approvalsList.Where(x => x.ApprovalStatus == ApprovalStatus.Approved).ToList();
                var denied = approvalsList.Where(x => x.ApprovalStatus == ApprovalStatus.Denied).ToList();

                if (dto.ApprovalStatus == ApprovalStatus.Denied || denied.Any())
                {
                    changeRequest.ChangeRequestStatus = ChangeRequestStatus.Blocked;
                }
                else if (approved.Count >= 1)
                {
                    changeRequest.ChangeRequestStatus = ChangeRequestStatus.Approved;
                }
                else
                {
                    changeRequest.ChangeRequestStatus = ChangeRequestStatus.Active;
                }
            }
        }
    }
}
