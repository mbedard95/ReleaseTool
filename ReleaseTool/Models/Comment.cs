using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models   
{
    [Table("Comments")]
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentData { get; set; } = "";
        public int UserId { get; set; }
        public int ChangeRequestId { get; set; }
        public DateTime Created { get; set; }
    }
}
