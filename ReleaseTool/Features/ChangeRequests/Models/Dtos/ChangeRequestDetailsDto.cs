using ReleaseTool.Features.Groups.Models.Dtos;
using ReleaseTool.Features.Tags.Models.Dtos;

namespace ReleaseTool.Features.ChangeRequests.Models.Dtos
{
    public class ChangeRequestDetailsDto
    {
        public Guid ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public List<ReadTagDto> Tags { get; set; } = new List<ReadTagDto>();
        public List<ReadGroupDto> UserGroups { get; set; } = new List<ReadGroupDto>();
    }
}
