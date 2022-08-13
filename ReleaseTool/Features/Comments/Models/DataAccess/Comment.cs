﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Comments.Models.DataAccess
{
    [Table("Comments")]
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentData { get; set; } = "";
        public int UserId { get; set; }
        public int ChangeRequestId { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }
}
