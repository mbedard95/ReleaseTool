using ReleaseTool.Features.Users.Models.DataAccess;

namespace ReleaseTool.Features.Users.Models.Dtos
{
    public class ReadUserDto
    {
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserProfile UserProfile { get; set; }
    }
}
