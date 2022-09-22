using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Comments.Models.DataAccess
{
    [Table("Comments")]
    public class Comment
    {
        public Guid CommentId { get; set; }
        public string CommentData { get; set; } = "";
        public Guid UserId { get; set; }
        public Guid ChangeRequestId { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}
