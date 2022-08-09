using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("ChangeRequests")]
    public class ChangeRequest
    {
        public int ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        public string NotifyEmails { get; set; } = "";
        public DateTime Created { get; set; }
        public int UserId { get; set; }
    }
}
