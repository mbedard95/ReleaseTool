using ReleaseTool.Features.Tags.Models.DataAccess;

namespace ReleaseTool.Features.Change_Requests.Models.Dtos
{
    public class WriteChangeRequestDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        public string NotifyEmails { get; set; } = "";
        public int UserId { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
