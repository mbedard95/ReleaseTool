using ReleaseTool.Features.Tags.Models.DataAccess;
using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Tags.Models.Dtos
{
    public class WriteTagDto
    {
        [Required]
        public string Name { get; set; } = "";
        public TagStatus TagStatus { get; set; } = TagStatus.Active;
    }
}
