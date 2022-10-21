using ReleaseTool.Features.Tags.Models.DataAccess;

namespace ReleaseTool.Features.Change_Requests.Models
{
    public class ReadChangeRequestDto
    {
        public Guid ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> UserGroups { get; set; } = new List<string>();
    }
}
