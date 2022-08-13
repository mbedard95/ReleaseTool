﻿using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Comments.Models.Dtos
{
    public class WriteCommentDto
    {
        [Required]
        public string CommentData { get; set; } = "";
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChangeRequestId { get; set; }
    }
}
