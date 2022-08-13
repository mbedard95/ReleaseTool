using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Approvals.Models.Dtos
{
    public class WriteApprovalDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChangeRequestId { get; set; }
        [Required]
        public string EmailAddress { get; set; } = "";
    }
}
