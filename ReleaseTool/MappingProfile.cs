using AutoMapper;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Comments.Models.DataAccess;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using ReleaseTool.Models;
using System.Text;
using XSystem.Security.Cryptography;

namespace ReleaseTool
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WriteUserDto, User>()
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => HashPassword(src.Password)));
            CreateMap<User, ReadUserDto>();
            
            CreateMap<WriteTagDto, Tag>();
            
            CreateMap<WriteCommentDto, Comment>();
            
            CreateMap<WriteApprovalDto, Approval>();
            
            CreateMap<WriteChangeRequestDto, ChangeRequest>();
            CreateMap<ChangeRequest, ReadChangeRequestDto>();
        }

        private string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }
    }
}
