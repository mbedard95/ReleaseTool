using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Groups.Models.Dtos;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.Dtos;

namespace ReleaseTool.Common
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public string Message { get; set; } = "";
    }
    public class RuleValidator : IRuleValidator
    {
        private readonly ReleaseToolContext _context;

        public RuleValidator(ReleaseToolContext context)
        {
            _context = context;
        }
        public ValidationResult IsValidUser(WriteUserDto dto, Guid id)
        {
            var result = new ValidationResult();
            if (EmailExists(dto.EmailAddress, id))
            {
                result.IsValid = false;
                result.Message = "Username already exists.";
                return result;
            }
            return result;
        }

        public ValidationResult IsValidChangeRequest(WriteChangeRequestDto dto)
        {
            var result = new ValidationResult();
            if (!UserExists(dto.UserId))
            {
                result.IsValid = false;
                result.Message = "User not found.";
                return result;
            }
            foreach (var tagName in dto.Tags)
            {
                if (!TagNameExists(tagName))
                {
                    result.IsValid = false;
                    result.Message = $"Tag \"{tagName}\" not found";
                    return result;
                }
            }
            return result;
        }

        public ValidationResult IsValidTag(WriteTagDto dto)
        {
            var result = new ValidationResult();
            if (TagNameExists(dto.Name))
            {
                result.IsValid = false;
                result.Message = "Tag name already exists.";
                return result;
            }
            return result;
        }

        public ValidationResult IsValidGroup(WriteGroupDto dto)
        {
            var result = new ValidationResult();
            if (GroupNameExists(dto.GroupName))
            {
                result.IsValid = false;
                result.Message = "Group name already exists.";
                return result;
            }
            return result;
        }

        public ValidationResult IsValidComment(WriteCommentDto dto)
        {
            var result = new ValidationResult();
            if (!ChangeRequestExists(dto.ChangeRequestId))
            {
                result.IsValid = false;
                result.Message = "Change request not found.";
                return result;
            }
            if (!UserExists(dto.UserId))
            {
                result.IsValid = false;
                result.Message = "User not found.";
                return result;
            }
            return result;
        }

        public ValidationResult IsValidApproval(WriteApprovalDto dto)
        {
            var result = new ValidationResult();
            if (!ChangeRequestExists(dto.ChangeRequestId))
            {
                result.IsValid = false;
                result.Message = "Change request not found.";
                return result;
            }
            if (!UserExists(dto.UserId))
            {
                result.IsValid = false;
                result.Message = "User not found.";
                return result;
            }
            return result;
        }

        private bool EmailExists(string email, Guid id)
        {
            return (_context.Users?.Any(x => x.EmailAddress == email && x.UserId != id)).GetValueOrDefault();
        }
        private bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        private bool TagNameExists(string name)
        {
            return (_context.Tags?.Any(x => x.Name== name)).GetValueOrDefault();
        }
        private bool GroupNameExists(string name)
        {
            return (_context.Groups?.Any(x => x.GroupName == name)).GetValueOrDefault();
        }
        private bool ChangeRequestExists(Guid id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }
    }
}
