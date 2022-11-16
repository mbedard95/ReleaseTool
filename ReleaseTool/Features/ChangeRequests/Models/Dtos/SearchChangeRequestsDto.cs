using ReleaseTool.Models;

namespace ReleaseTool.Features.ChangeRequests.Models.Dtos
{
    public class SearchChangeRequestsDto
    {
        public string? Title { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool AssignedToActiveUser { get; set; }
        public List<ChangeRequestStatus> Statuses { get; set; } = new List<ChangeRequestStatus>();
        public List<string> Tags { get; set; } = new List<string>();
    }
}
