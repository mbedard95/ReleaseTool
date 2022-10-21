using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;

namespace ReleaseTool.Features.Approvals
{
    public interface IApprovalsProvider
    {
        Task AddNewApproval(WriteApprovalDto dto);
    }

    public class ApprovalsProvider : IApprovalsProvider
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public ApprovalsProvider(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task AddNewApproval(WriteApprovalDto dto)
        {
            var id = Guid.NewGuid();
            var validationResult = _validator.IsValidApproval(dto);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.Message);
            }

            var approval = CreateApproval(dto, id);

            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();
        }

        private Approval CreateApproval(WriteApprovalDto dto, Guid id)
        {
            var approval = _mapper.Map<Approval>(dto);
            approval.ApprovalStatus = ApprovalStatus.Pending;
            approval.ApprovedDate = DateTime.MaxValue;
            approval.Created = DateTime.Now;

            return approval;
        }
    }
}
