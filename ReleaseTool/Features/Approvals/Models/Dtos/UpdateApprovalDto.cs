using ReleaseTool.Features.Approvals.Models.DataAccess;

namespace ReleaseTool.Features.Approvals.Models.Dtos
{
    public class UpdateApprovalDto
    {
        public ApprovalStatus ApprovalStatus { get; set; }
        public Guid UserId { get; set; }
    }
}
