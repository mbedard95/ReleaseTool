using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Groups.Models.Dtos
{
    public class WriteGroupDto
    {
        [Required]
        public string GroupName { get; set; } = "";
    }
}
