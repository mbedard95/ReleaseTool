using ReleaseTool.Features.Users.Models.DataAccess;

namespace ReleaseTool.Features.Users.Models.Dtos
{
    public class ReadUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserStatus UserStatus { get; set; }
        public string EmailAddress { get; set; } = "";
        public DateTime Created { get; set; }
        public UserPermissions UserPermissions { get; set; }
    }
}
