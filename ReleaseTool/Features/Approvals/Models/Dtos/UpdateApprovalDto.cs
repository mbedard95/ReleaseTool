using ReleaseTool.Features.Approvals.Models.DataAccess;

namespace ReleaseTool.Features.Approvals.Models.Dtos
{
    public class UpdateApprovalDto
    {
        public ApprovalStatus ApprovalStatus { get; set; }
        public string EmailAddress { get; set; } = "";
    }
}
