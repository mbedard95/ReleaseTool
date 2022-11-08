using ReleaseTool.Features.Approvals.Models.DataAccess;

namespace ReleaseTool.Features.Approvals.Models.Dtos
{
    public class ReadApprovalDto
    {
        public Guid ApprovalId { get; set; }
        public Guid UserId { get; set; }
        public Guid ChangeRequestId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public ApprovalStatus ApprovalStatus { get; set; }
        public string EmailAddress { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
