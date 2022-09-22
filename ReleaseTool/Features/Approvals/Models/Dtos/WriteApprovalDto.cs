using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Approvals.Models.Dtos
{
    public class WriteApprovalDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ChangeRequestId { get; set; }
        [Required]
        public string EmailAddress { get; set; } = "";
    }
}
