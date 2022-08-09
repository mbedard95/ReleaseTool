using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using ReleaseTool.Models;

namespace ReleaseTool.Models
{
    public class ReleaseToolContext : DbContext
    {
        public ReleaseToolContext(DbContextOptions<ReleaseToolContext> options)
            : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ReleaseTool.Models.Approval>? Approval { get; set; }
        public DbSet<ReleaseTool.Models.Tag>? Tag { get; set; }
        public DbSet<ReleaseTool.Models.Comment>? Comment { get; set; }
        public DbSet<ReleaseTool.Models.ChangeRequest>? ChangeRequest { get; set; }
        public DbSet<ReleaseTool.Models.ChangeRequestTag>? ChangeRequestTag { get; set; }
    }
}
