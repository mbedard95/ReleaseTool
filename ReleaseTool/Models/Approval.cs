using ReleaseTool.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("Approvals")]
    public class Approval
    {
        public int ApprovalId { get; set; }
        public int UserId { get; set; }
        public int ChangeRequestId { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string EmailAddress { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
