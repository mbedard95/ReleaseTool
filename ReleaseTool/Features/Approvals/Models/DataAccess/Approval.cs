using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Approvals.Models.DataAccess
{
    [Table("Approvals")]
    public class Approval
    {
        public int ApprovalId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChangeRequestId { get; set; }
        [Required]
        public ApprovalStatus ApprovalStatus { get; set; }
        [Required]
        public string EmailAddress { get; set; } = "";
        [Required]
        public DateTime Created { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
