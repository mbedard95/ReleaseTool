using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Tags.Models.DataAccess
{
    [Table("ChangeRequestTags")]
    public class ChangeRequestTag
    {
        public Guid ChangeRequestTagId { get; set; }
        [Required]
        public Guid ChangeRequestId { get; set; }
        [Required]
        public Guid TagId { get; set; }
    }
}
