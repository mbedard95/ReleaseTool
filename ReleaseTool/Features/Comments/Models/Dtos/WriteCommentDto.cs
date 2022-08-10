using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Comments.Models.Dtos
{
    public class WriteCommentDto
    {
        [StringLength(8000, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string CommentData { get; set; } = "";
        public int UserId { get; set; }
        public int ChangeRequestId { get; set; }
    }
}
