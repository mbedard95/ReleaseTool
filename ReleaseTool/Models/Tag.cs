using ReleaseTool.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("Tags")]
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; } = "";
        public DateTime Created { get; set; }
        public TagStatus TagStatus { get; set; }
    }
}
