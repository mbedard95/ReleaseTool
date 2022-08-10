using AutoMapper;
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
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => HashPassword(src.Password))); ;

            CreateMap<User, ViewUserDto>();
        }

        private string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }
    }
}
