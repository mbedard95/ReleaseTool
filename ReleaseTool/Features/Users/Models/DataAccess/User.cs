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
        public UserProfile UserProfile { get; set; }
        public bool IsActiveUser { get; set; }
    }


    public enum UserProfile
    {
        ReadOnly,
        ReadAndWriteOnly,
        Approver,
        Admin
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
