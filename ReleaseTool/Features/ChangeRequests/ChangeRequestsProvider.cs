using AutoMapper;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.DataAccess;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Models;

namespace ReleaseTool.Features.ChangeRequests
{
    public interface IChangeRequestsProvider
    {
        bool ChangeRequestExists(Guid id);
        ReadChangeRequestDto ConvertToView(ChangeRequest changeRequest);
        List<string> GetGroupNames(Guid changeRequestId);
        ChangeRequest GetNewChangeRequest(WriteChangeRequestDto dto);
        List<string> GetTagNames(Guid changeRequestId);
        void SaveChangeRequestGroupMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest);
        void SaveChangeRequestTagMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest);
    }

    public class ChangeRequestsProvider : IChangeRequestsProvider
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;

        public ChangeRequestsProvider(ReleaseToolContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool ChangeRequestExists(Guid id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }

        public List<string> GetTagNames(Guid changeRequestId)
        {
            List<string> tagNames = new();
            var tagMaps = _context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequestId).ToList();
            foreach (var tag in tagMaps)
            {
                var tagObject = _context.Tags.FirstOrDefault(x => x.TagId == tag.TagId);
                if (tagObject != null)
                {
                    tagNames.Add(tagObject.Name);
                }
            }
            return tagNames;
        }

        public List<string> GetGroupNames(Guid changeRequestId)
        {
            List<string> groupNames = new();
            var groupMaps = _context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequestId).ToList();
            foreach (var group in groupMaps)
            {
                var groupObject = _context.Groups.FirstOrDefault(x => x.GroupId == group.GroupId);
                if (groupObject != null)
                {
                    groupNames.Add(groupObject.GroupName);
                }
            }
            return groupNames;
        }

        public void SaveChangeRequestTagMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest)
        {
            _context.RemoveRange(_context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequest.ChangeRequestId));
            foreach (var tagName in dto.Tags)
            {
                var tag = _context.Tags.FirstOrDefault(x => x.Name == tagName);
                if (tag != null)
                {
                    _context.ChangeRequestTags.Add(new ChangeRequestTag
                    {
                        ChangeRequestId = changeRequest.ChangeRequestId,
                        TagId = tag.TagId
                    });
                }
            }
        }

        public void SaveChangeRequestGroupMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest)
        {
            _context.RemoveRange(_context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequest.ChangeRequestId));
            foreach (var groupName in dto.UserGroups)
            {
                var group = _context.Groups.FirstOrDefault(x => x.GroupName == groupName);
                if (group != null)
                {
                    _context.ChangeRequestGroups.Add(new ChangeRequestGroup
                    {
                        ChangeRequestId = changeRequest.ChangeRequestId,
                        GroupId = group.GroupId
                    });
                }
            }
        }

        public ReadChangeRequestDto ConvertToView(ChangeRequest changeRequest)
        {
            var result = _mapper.Map<ReadChangeRequestDto>(changeRequest);
            result.Tags = GetTagNames(result.ChangeRequestId);
            result.UserGroups = GetGroupNames(result.ChangeRequestId);
            return result;
        }

        public ChangeRequest GetNewChangeRequest(WriteChangeRequestDto dto)
        {
            var changeRequest = _mapper.Map<ChangeRequest>(dto);
            changeRequest.Created = DateTime.Now;
            changeRequest.ChangeRequestStatus = ChangeRequestStatus.Active;

            return changeRequest;
        }
    }
}
