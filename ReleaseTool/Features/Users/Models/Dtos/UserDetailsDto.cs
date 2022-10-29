using ReleaseTool.Features.Users.Models.DataAccess;

namespace ReleaseTool.Features.Users.Models.Dtos
{
    public class UserDetailsDto
    {
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserStatus UserStatus { get; set; }
        public DateTime Created { get; set; }
        public UserProfile UserProfile { get; set; }
        public List<string> Groups { get; set; } = new List<string>();
    }
}
