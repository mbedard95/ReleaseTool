using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Groups.Models.DataAccess
{
    [Table("Groups")]
    public class Group
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = "";
        public DateTime Created { get; set; }
        public GroupStatus GroupStatus { get; set; }
    }

    public enum GroupStatus
    {
        Active,
        Inactive
    }
}
