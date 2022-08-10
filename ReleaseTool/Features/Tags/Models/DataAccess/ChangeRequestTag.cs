using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Tags.Models.DataAccess
{
    [Table("ChangeRequestTags")]
    public class ChangeRequestTag
    {
        public int ChangeRequestTagId { get; set; }
        [Required]
        public int ChangeRequestId { get; set; }
        [Required]
        public int TagId { get; set; }
    }
}
