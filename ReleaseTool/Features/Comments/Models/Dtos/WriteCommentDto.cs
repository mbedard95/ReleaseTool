using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Comments.Models.Dtos
{
    public class WriteCommentDto
    {
        [Required]
        public string CommentData { get; set; } = "";
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ChangeRequestId { get; set; }
    }
}
