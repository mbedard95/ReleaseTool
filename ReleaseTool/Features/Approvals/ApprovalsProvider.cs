using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using ReleaseTool.Common;
using ReleaseTool.Common.Email;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Approvals.Models.DataAccess;
using ReleaseTool.Features.Approvals.Models.Dtos;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Users.Models.DataAccess;

namespace ReleaseTool.Features.Approvals
{
    public interface IApprovalsProvider
    {
        Task AddNewApprovalAsync(WriteApprovalDto dto);
        Task SendApprovalEmailAsync(User user, WriteChangeRequestDto dto);
    }

    public class ApprovalsProvider : IApprovalsProvider
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;
        private readonly EmailConfiguration _config;

        public ApprovalsProvider(
            ReleaseToolContext context, 
            IMapper mapper, 
            IRuleValidator validator,
            EmailConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _config = config;
        }

        public async Task AddNewApprovalAsync(WriteApprovalDto dto)
        {
            var id = Guid.NewGuid();
            var validationResult = _validator.IsValidApproval(dto, true);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.Message);
            }

            var approval = CreateApproval(dto, id);

            await _context.Approvals.AddAsync(approval);
        }

        public async Task SendApprovalEmailAsync(User user, WriteChangeRequestDto dto)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_config.From)
            };
            email.To.Add(MailboxAddress.Parse(user.EmailAddress));
            email.Subject = $"New Change Request: {dto.Title}";
            var builder = new BodyBuilder
            {
                HtmlBody = $"<p>Hi {user.FirstName} {user.LastName},</p>" +
                $"<p>You have a new change request to review. Please <a href='http://localhost:8080/#/viewchangerequests'>click here</a> to view it.</p>" +
                $"<h3>Title</h3>" +
                $"<p>{dto.Title}</p>" +
                $"<h3>Description</h3>" +
                $@"<p style=""white-space: pre-line;"">{dto.Description}</p>" +
                $"<h3>Release Steps</h3>" +
                $@"<p style=""white-space: pre-line;"">{dto.ReleaseSteps}</p>" +
                $"<h3>Rollback Procedure</h3>" +
                $@"<p style=""white-space: pre-line;"">{dto.RollbackProcedure}</p>" +
                $"<p>Regards,</p>" +
                $"Capstone Release Tool"
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_config.SmtpServer, _config.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.UserName, _config.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        private Approval CreateApproval(WriteApprovalDto dto, Guid id)
        {
            var approval = _mapper.Map<Approval>(dto);
            approval.ApprovalStatus = ApprovalStatus.Pending;
            approval.ApprovedDate = DateTime.MaxValue;
            approval.Created = DateTime.UtcNow;

            return approval;
        }
    }
}
