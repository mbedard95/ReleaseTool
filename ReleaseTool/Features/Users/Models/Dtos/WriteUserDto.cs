using ReleaseTool.Features.Users.Models.DataAccess;
using System.ComponentModel.DataAnnotations;

namespace ReleaseTool.Features.Users.Models.Dtos
{
    public class WriteUserDto
    {
        [StringLength(128, ErrorMessage = "{0} length must be between {2} and {1} characters.", MinimumLength = 5)]
        public string EmailAddress { get; set; } = "";
        [StringLength(32, ErrorMessage = "{0} length must be between {2} and {1} characters.", MinimumLength = 8)]
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserPermissions UserPermissions { get; set; }
    }
}
