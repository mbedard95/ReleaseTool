using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Users.Models.DataAccess
{
    [Table("Users")]
    public class User
    {
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        [Required]
        public UserStatus UserStatus { get; set; }       
        [Required]
        public DateTime Created { get; set; }
        public UserPermissions UserPermissions { get; set; }
    }

    [Flags]
    public enum UserPermissions
    {
        ReadRequest = 1,
        CommentRequest = 2,
        WriteRequest = 4,
        ApproveRequest = 8,
        ManageUsers = 16
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
