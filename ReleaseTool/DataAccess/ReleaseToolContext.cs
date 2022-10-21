﻿using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using ReleaseTool.Models;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Comments.Models.DataAccess;
using ReleaseTool.Features.Groups.Models.DataAccess;
using ReleaseTool.Features.Change_Requests.Models.DataAccess;

namespace ReleaseTool.DataAccess
{
    public class ReleaseToolContext : DbContext
    {
        public ReleaseToolContext(DbContextOptions<ReleaseToolContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Approval>? Approvals { get; set; }
        public DbSet<Tag>? Tags { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<ChangeRequest>? ChangeRequests { get; set; }
        public DbSet<ChangeRequestTag>? ChangeRequestTags { get; set; }
        public DbSet<Group>? Groups { get; set; }
        public DbSet<UserGroup>? UserGroups { get; set; }
        public DbSet<ChangeRequestGroup>? ChangeRequestGroups { get; set; }
    }
}
