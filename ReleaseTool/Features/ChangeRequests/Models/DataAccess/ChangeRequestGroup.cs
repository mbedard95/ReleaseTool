using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Change_Requests.Models.DataAccess
{
    [Table("ChangeRequestGroups")]
    public class ChangeRequestGroup
    {
        public Guid ChangeRequestGroupId { get; set; }
        [Required]
        public Guid ChangeRequestId { get; set; }
        [Required]
        public Guid GroupId { get; set; }
    }
}
