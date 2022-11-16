using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Models;

namespace ReleaseTool.Features.Change_Requests.Models
{
    public class ReadChangeRequestDto
    {
        public Guid ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public string UserDisplayName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public ChangeRequestStatus ChangeRequestStatus { get; set; }
    }
}
