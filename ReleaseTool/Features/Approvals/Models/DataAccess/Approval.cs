using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Approvals.Models.DataAccess
{
    [Table("Approvals")]
    public class Approval
    {
        public Guid ApprovalId { get; set; }
        public Guid UserId { get; set; }
        public Guid ChangeRequestId { get; set; }
        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }
        public string EmailAddress { get; set; } = "";
        [Required]
        public DateTime Created { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Denied,
        Removed
    }
}
