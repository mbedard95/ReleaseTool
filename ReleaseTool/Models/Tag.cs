using ReleaseTool.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
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
}
