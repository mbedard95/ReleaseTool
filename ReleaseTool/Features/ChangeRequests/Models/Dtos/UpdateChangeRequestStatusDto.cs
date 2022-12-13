using ReleaseTool.Models;

namespace ReleaseTool.Features.ChangeRequests.Models.Dtos
{
    public class UpdateChangeRequestStatusDto
    {
        public ChangeRequestStatus ChangeRequestStatus { get; set; }
        public Guid UserId { get; set; }
    }
}
