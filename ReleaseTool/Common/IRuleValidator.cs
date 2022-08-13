using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.Dtos;

namespace ReleaseTool.Common
{
    public interface IRuleValidator
    {
        ValidationResult IsValidUser(WriteUserDto dto);
        ValidationResult IsValidChangeRequest(WriteChangeRequestDto dto);
        ValidationResult IsValidTag(WriteTagDto dto);
        ValidationResult IsValidComment(WriteCommentDto dto);
        ValidationResult IsValidApproval(WriteApprovalDto dto);
    }
}