using ReleaseTool.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReleaseTool.Models
{
    [Table("Users")]
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserStatus UserStatus { get; set; }
        public string EmailAddress { get; set; } = "";
        public DateTime Created { get; set; }
        public UserPermissions UserPermissions { get; set; }
    }
}
