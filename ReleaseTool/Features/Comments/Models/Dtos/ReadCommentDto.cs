namespace ReleaseTool.Features.Comments.Models.Dtos
{
    public class ReadCommentDto
    {
        public Guid CommentId { get; set; }
        public string CommentData { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public Guid ChangeRequestId { get; set; }
        public DateTime Created { get; set; }
    }
}
