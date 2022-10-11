using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Users.Models.DataAccess
{
    [Table("UserGroups")]
    public class UserGroup
    {
        public Guid UserGroupId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid GroupId { get; set; }
    }
}
