using System.Text;
using AutoMapper;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using XSystem.Security.Cryptography;

namespace ReleaseTool.Features.Users
{
    public interface IUsersProvider
    {
        ReadUserDto ConvertToView(User user);
        List<string> GetGroupNames(Guid userId);
        User GetNewUser(WriteUserDto dto, Guid id);
        void SaveUserGroupMaps(WriteUserDto dto, User user);
        bool UserExists(Guid id);
        string GetDisplayName(Guid id);
        UserDetailsDto ConvertToDetailsView(User user);
    }

    public class UsersProvider : IUsersProvider
    {
        private readonly IMapper _mapper;
        private readonly ReleaseToolContext _context;

        public UsersProvider(IMapper mapper, ReleaseToolContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public User GetNewUser(WriteUserDto dto, Guid id)
        {
            var newUser = _mapper.Map<User>(dto);
            newUser.Password = HashPassword(dto.Password);

            newUser.UserId = id;
            newUser.UserStatus = UserStatus.Active;
            newUser.Created = DateTime.UtcNow;

            return newUser;
        }

        public bool UserExists(Guid id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        public static string HashPassword(string pw)
        {
            var hash = new SHA256Managed();
            byte[] crypto = hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            return Convert.ToBase64String(crypto);
        }

        public List<string> GetGroupNames(Guid userId)
        {
            List<string> groupNames = new();

            var groupMaps = _context.UserGroups.Where(x => x.UserId == userId).ToList();
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

        public void SaveUserGroupMaps(WriteUserDto dto, User user)
        {
            _context.RemoveRange(_context.UserGroups.Where(x => x.UserId == user.UserId));
            foreach (var groupName in dto.Groups)
            {
                var group = _context.Groups.FirstOrDefault(x => x.GroupName == groupName && x.GroupStatus == GroupStatus.Active);
                if (group != null)
                {
                    _context.UserGroups.Add(new UserGroup
                    {
                        UserId = user.UserId,
                        GroupId = group.GroupId
                    });
                }
            }
        }

        public ReadUserDto ConvertToView(User user)
        {
            var result = _mapper.Map<ReadUserDto>(user);
            return result;
        }

        public UserDetailsDto ConvertToDetailsView(User user)
        {
            var result = _mapper.Map<UserDetailsDto>(user);
            result.Groups = GetGroupsForUser(user.UserId);
            return result;
        }

        public string GetDisplayName(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return "Unknown User";
            }
            return $"{user.FirstName} {user.LastName}";
        }

        private List<string> GetGroupsForUser(Guid userId)
        {
            var groupIds = _context.UserGroups.Where(x => x.UserId == userId)
                .Select(x => x.GroupId).ToList();
            return _context.Groups.Where(x => groupIds.Contains(x.GroupId))
                .Select(x => x.GroupName).ToList();
        }
    }
}
