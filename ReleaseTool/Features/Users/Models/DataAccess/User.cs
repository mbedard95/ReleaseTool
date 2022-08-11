using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Features.Users.Models.DataAccess
{
    [Table("Users")]
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string EmailAddress { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        [Required]
        public UserStatus UserStatus { get; set; }       
        [Required]
        public DateTime Created { get; set; }
        [Required]
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
