using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Comments.Models.Dtos;
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
        public ValidationResult IsValidUser(WriteUserDto dto)
        {
            var result = new ValidationResult();
            if (EmailExists(dto.EmailAddress))
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

        private bool EmailExists(string email)
        {
            return (_context.Users?.Any(x => x.EmailAddress == email)).GetValueOrDefault();
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        private bool TagNameExists(string name)
        {
            return (_context.Tags?.Any(x => x.Name== name)).GetValueOrDefault();
        }
        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }
    }
}
