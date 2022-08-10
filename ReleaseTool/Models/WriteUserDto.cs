using ReleaseTool.Models.Enums;

namespace ReleaseTool.Models
{
    public class WriteUserDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public UserPermissions UserPermissions { get; set; }
    }
}
