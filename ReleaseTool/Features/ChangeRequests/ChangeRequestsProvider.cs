using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models.DataAccess;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.ChangeRequests.Models.Dtos;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Groups.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Models;

namespace ReleaseTool.Features.ChangeRequests
{
    public interface IChangeRequestsProvider
    {
        bool ChangeRequestExists(Guid id);
        ChangeRequestDetailsDto ConvertToDetailsView(ChangeRequest changeRequest);
        ChangeRequest GetNewChangeRequest(WriteChangeRequestDto dto);
        List<ReadTagDto> GetTagNames(Guid changeRequestId);
        void SaveChangeRequestGroupMaps(WriteChangeRequestDto dto, Guid changeRequestId);
        void SaveChangeRequestTagMaps(WriteChangeRequestDto dto, Guid changeRequestId);
        Task MergeApprovals(WriteChangeRequestDto dto, Guid changeRequestId);
    }

    public class ChangeRequestsProvider : IChangeRequestsProvider
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IApprovalsProvider _approvalsProvider;

        public ChangeRequestsProvider(ReleaseToolContext context, IMapper mapper, IApprovalsProvider approvalsProvider)
        {
            _context = context;
            _mapper = mapper;
            _approvalsProvider = approvalsProvider;
        }

        public bool ChangeRequestExists(Guid id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }

        public List<ReadTagDto> GetTagNames(Guid changeRequestId)
        {
            List<ReadTagDto> tagNames = new();
            var tagMaps = _context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequestId).ToList();
            foreach (var tag in tagMaps)
            {
                var tagObject = _context.Tags.FirstOrDefault(x => x.TagId == tag.TagId);
                if (tagObject != null && tagObject.TagStatus == TagStatus.Active)
                {
                    tagNames.Add(new ReadTagDto
                    {
                        TagId = tagObject.TagId,
                        TagName = tagObject.Name
                    });
                }
            }
            return tagNames;
        }

        public List<ReadGroupDto> GetGroupNames(Guid changeRequestId)
        {
            List<ReadGroupDto> groupNames = new();
            var groupMaps = _context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequestId).ToList();
            foreach (var group in groupMaps)
            {
                var groupObject = _context.Groups.FirstOrDefault(x => x.GroupId == group.GroupId);
                if (groupObject != null && groupObject.GroupStatus == GroupStatus.Active)
                {
                    groupNames.Add(new ReadGroupDto
                    {
                        GroupId = groupObject.GroupId,
                        GroupName = groupObject.GroupName
                    });
                }
            }
            return groupNames;
        }

        public void SaveChangeRequestTagMaps(WriteChangeRequestDto dto, Guid changeRequestId)
        {
            _context.RemoveRange(_context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequestId));
            foreach (var tagName in dto.Tags)
            {
                var tag = _context.Tags.FirstOrDefault(x => x.Name == tagName && x.TagStatus == TagStatus.Active);
                if (tag != null)
                {
                    _context.ChangeRequestTags.Add(new ChangeRequestTag
                    {
                        ChangeRequestId = changeRequestId,
                        TagId = tag.TagId
                    });
                }
            }
        }

        public void SaveChangeRequestGroupMaps(WriteChangeRequestDto dto, Guid changeRequestId)
        {
            _context.RemoveRange(_context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequestId));
            foreach (var groupName in dto.UserGroups)
            {
                var group = _context.Groups.FirstOrDefault(x => x.GroupName == groupName && x.GroupStatus == GroupStatus.Active);
                if (group != null)
                {
                    _context.ChangeRequestGroups.Add(new ChangeRequestGroup
                    {
                        ChangeRequestId = changeRequestId,
                        GroupId = group.GroupId
                    });
                }
            }
        }

        public ChangeRequestDetailsDto ConvertToDetailsView(ChangeRequest changeRequest)
        {
            var result = _mapper.Map<ChangeRequestDetailsDto>(changeRequest);
            result.Tags = GetTagNames(result.ChangeRequestId);
            result.Approvals = GetApprovals(result.ChangeRequestId);
            return result;
        }

        public ChangeRequest GetNewChangeRequest(WriteChangeRequestDto dto)
        {
            var changeRequest = _mapper.Map<ChangeRequest>(dto);
            changeRequest.Created = DateTime.UtcNow;
            changeRequest.ChangeRequestStatus = ChangeRequestStatus.Active;

            return changeRequest;
        }

        public async Task MergeApprovals(WriteChangeRequestDto dto, Guid changeRequestId)
        {
            var groupIds = _context.Groups.Where(x => x.GroupStatus == GroupStatus.Active && dto.UserGroups.Contains(x.GroupName)).Select(x => x.GroupId).ToList();
            var userIds = _context.UserGroups.Where(x => groupIds.Contains(x.GroupId)).Select(x => x.UserId).Distinct().ToList();
            var users = _context.Users.Where(x => x.UserStatus == UserStatus.Active && userIds.Contains(x.UserId)).ToList();

            var existingApprovals = _context.Approvals.Where(x => x.ChangeRequestId == changeRequestId && x.ApprovalStatus != ApprovalStatus.Removed).ToList();
            var usersToRemove = existingApprovals.Select(x => x.UserId).Where(x => !users.Select(x => x.UserId).Contains(x));
            var approvalsToRemove = existingApprovals.Where(x => usersToRemove.Contains(x.UserId));

            foreach (var approval in approvalsToRemove)
            {
                approval.ApprovalStatus = ApprovalStatus.Removed;
                _context.Entry(approval).State = EntityState.Modified;
            }

            foreach (var user in users)
            {
                var existingApproval = _context.Approvals.Where(x => x.UserId == user.UserId && x.ChangeRequestId == changeRequestId && x.ApprovalStatus != ApprovalStatus.Removed).ToList();

                if (!existingApproval.Any())
                {
                    await _approvalsProvider.AddNewApprovalAsync(new WriteApprovalDto
                    {
                        UserId = user.UserId,
                        ChangeRequestId = changeRequestId,
                        EmailAddress = user.EmailAddress
                    });
                    await _approvalsProvider.SendApprovalEmailAsync(user, dto);
                }              
            }
        }

        private List<ReadApprovalDto> GetApprovals(Guid changeRequestId)
        {
            var approvalsList = new List<ReadApprovalDto>();

            var approvals = _context.Approvals
                .Where(x => x.ChangeRequestId == changeRequestId 
                && x.ApprovalStatus != ApprovalStatus.Removed).ToList();
            foreach (var approval in approvals)
            {
                var dto = _mapper.Map<ReadApprovalDto>(approval);
                var user = _context.Users.Find(approval.UserId);
                if (user != null)
                {
                    dto.FirstName = user.FirstName;
                    dto.LastName = user.LastName;                   
                }  
                else
                {
                    dto.FirstName = "Unknown";
                    dto.LastName = "User";
                }
                approvalsList.Add(dto);
            }
            return approvalsList
                .OrderBy(x => x.ApprovalStatus)
                .ThenBy(x => x.LastName).ToList();
        }
    }
}
