using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Groups.Models.Dtos;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Models;

namespace ReleaseTool.Features.ChangeRequests.Models.Dtos
{
    public class ChangeRequestDetailsDto
    {
        public Guid ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        public ChangeRequestStatus ChangeRequestStatus { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public Guid? DeployedUserId { get; set; }
        public List<ReadTagDto> Tags { get; set; } = new List<ReadTagDto>();
        public List<ReadApprovalDto> Approvals { get; set; } = new List<ReadApprovalDto>();
        public List<ReadGroupDto> UserGroups { get; set; } = new List<ReadGroupDto>(); 
    }
}
