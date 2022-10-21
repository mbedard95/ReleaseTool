using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("ChangeRequests")]
    public class ChangeRequest
    {
        public Guid ChangeRequestId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        [Required]
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }
        public ChangeRequestStatus ChangeRequestStatus { get; set; }
    }

    public enum ChangeRequestStatus
    {
        Active,
        Complete,
        Abandoned
    }
}
