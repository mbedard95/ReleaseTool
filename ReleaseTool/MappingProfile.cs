using AutoMapper;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.ChangeRequests.Models.Dtos;
using ReleaseTool.Features.Comments.Models.DataAccess;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Groups.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using ReleaseTool.Models;

namespace ReleaseTool
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WriteUserDto, User>()
                .ForMember(dest => dest.Password, opts => opts.Ignore());
            CreateMap<User, ReadUserDto>();
            CreateMap<User, UserDetailsDto>();
            
            CreateMap<WriteTagDto, Tag>();
            
            CreateMap<WriteCommentDto, Comment>();
            CreateMap<Comment, ReadCommentDto>();
            
            CreateMap<WriteApprovalDto, Approval>();
            CreateMap<UpdateApprovalDto, Approval>();
            CreateMap<Approval, ReadApprovalDto>();
            
            CreateMap<WriteChangeRequestDto, ChangeRequest>();
            CreateMap<ChangeRequest, ReadChangeRequestDto>()
                .ForMember(dest => dest.UserDisplayName, opts => opts.Ignore());
            CreateMap<ChangeRequest, ChangeRequestDetailsDto>();

            CreateMap<WriteGroupDto, Group>();
        }
    }
}
