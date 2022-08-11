using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Tags.Models.DataAccess
{
    [Table("Tags")]
    public class Tag
    {
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public TagStatus TagStatus { get; set; }
    }

    public enum TagStatus
    {
        Active,
        Inactive
    }
}
