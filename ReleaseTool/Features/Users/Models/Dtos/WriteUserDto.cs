using ReleaseTool.Features.Users.Models.DataAccess;

namespace ReleaseTool.Features.Users.Models.Dtos
{
    public class WriteUserDto
    {
        public string EmailAddress { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserPermissions UserPermissions { get; set; }
    }
}
