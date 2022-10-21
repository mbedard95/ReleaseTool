using ReleaseTool.Features.Tags.Models.DataAccess;
using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Change_Requests.Models.Dtos
{
    public class WriteChangeRequestDto
    {
        [Required]
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReleaseSteps { get; set; } = "";
        public string RollbackProcedure { get; set; } = "";
        [Required]
        public Guid UserId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> UserGroups { get; set; } = new List<string>();
    }
}
