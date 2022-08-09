using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("ChangeRequestTags")]
    public class ChangeRequestTag
    {
        public int ChangeRequestTagId { get; set; }
        public int ChangeRequestId { get; set; }
        public int TagId { get; set; }
    }
}
