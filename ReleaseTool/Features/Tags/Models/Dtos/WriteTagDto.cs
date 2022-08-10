using ReleaseTool.Features.Tags.Models.DataAccess;

namespace ReleaseTool.Features.Tags.Models.Dtos
{
    public class WriteTagDto
    {
        public string Name { get; set; } = "";
        public TagStatus TagStatus { get; set; }
    }
}
