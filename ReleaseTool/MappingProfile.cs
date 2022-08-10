using AutoMapper;
using ReleaseTool.Features.Comments.Models.DataAccess;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.Dtos;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using System.Text;
using XSystem.Security.Cryptography;

namespace ReleaseTool
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WriteUserDto, User>()
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => HashPassword(src.Password))); ;

            CreateMap<User, ReadUserDto>();

            CreateMap<WriteTagDto, Tag>();

            CreateMap<WriteCommentDto, Comment>();
        }

        private string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }
    }
}
