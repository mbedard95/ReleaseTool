using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.ChangeRequests;
using ReleaseTool.Features.ChangeRequests.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
using ReleaseTool.Features.Users;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Models;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangeRequestsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;
        private readonly IChangeRequestsProvider _changeRequestsProvider;
        private readonly IUsersProvider _usersProvider;

        public ChangeRequestsController(
            ReleaseToolContext context, 
            IMapper mapper, 
            IRuleValidator validator,
            IChangeRequestsProvider changeRequestProvider,
            IUsersProvider usersProvider)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _changeRequestsProvider = changeRequestProvider;
            _usersProvider = usersProvider;
        }

        // GET: api/ChangeRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadChangeRequestDto>>> GetChangeRequests(bool includeInactive)
        {
            var changeRequests = await _context.ChangeRequests.ToListAsync();

            var mapped = includeInactive == true ?
                changeRequests.Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList()
                : changeRequests.Where(x => x.ChangeRequestStatus != ChangeRequestStatus.Abandoned).ToList()
                .Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList();

            foreach (var changeRequest in mapped)
            {
                
            }
            mapped.ForEach(x =>
            {
                var attributes = _usersProvider.GetUserAttributes(x.UserId);
                x.UserDisplayName = attributes.DisplayName;
                x.UserEmail = attributes.Email;
            });

            return mapped.OrderByDescending(x => x.Created).ToList();
        }

        // POST: api/SearchChangeRequests
        [HttpPost("SearchChangeRequests")]
        public async Task<ActionResult<IEnumerable<ReadChangeRequestDto>>> SearchChangeRequests(SearchChangeRequestsDto dto)
        {
            var changeRequests = await _context.ChangeRequests.ToListAsync();

            var mapped = changeRequests.Select(x => _mapper.Map<ReadChangeRequestDto>(x)).AsEnumerable();

            if (dto.Statuses.Any())
            {
                mapped = mapped.Where(x => dto.Statuses.Contains(x.ChangeRequestStatus));
            }

            if (dto.Title != null)
            {
                mapped = mapped.Where(x => x.Title.Contains(dto.Title));
            }

            if (dto.Tags.Any())
            {
                foreach (var tag in dto.Tags)
                {
                    var tagId = _context.Tags.Where(x => x.Name == tag && x.TagStatus != TagStatus.Inactive).Single().TagId;
                    var tagMaps = _context.ChangeRequestTags.Where(x => x.TagId == tagId && mapped.Select(x => x.ChangeRequestId).Contains(x.ChangeRequestId)).ToList();
                    mapped = mapped.Where(x => tagMaps.Where(y => y.ChangeRequestId == x.ChangeRequestId).Any());
                }
            }

            if (dto.StartDate != null)
            {
                mapped = mapped.Where(x => x.Created > dto.StartDate);
            }
            if (dto.EndDate != null)
            {
                mapped = mapped.Where(x => x.Created < dto.EndDate);
            }
            if (dto.AssignedToActiveUser)
            {
                var activeUser = _context.Users.FirstOrDefault(x => x.IsActiveUser);
                if (activeUser == null)
                {
                    throw new Exception("Active user not selected");
                }
                var changeRequestIdsForUser = _context.Approvals.Where(x => x.EmailAddress == activeUser.EmailAddress).Select(x => x.ChangeRequestId).ToList();
                mapped = mapped.Where(x => changeRequestIdsForUser.Contains(x.ChangeRequestId));
            }

            var mappedList = mapped.ToList();
            mappedList.ForEach(x =>
            {
                var attributes = _usersProvider.GetUserAttributes(x.UserId);
                x.UserDisplayName = attributes.DisplayName;
                x.UserEmail = attributes.Email;
            });
            if (dto.Email != null)
            {
                mappedList = mappedList.Where(x => x.UserEmail.Contains(dto.Email)).ToList();
            }            

            return mappedList.OrderByDescending(x => x.Created).ToList();
        }

        // GET: api/ChangeRequests/5
        [HttpGet("Details/{id}")]
        public async Task<ActionResult<ChangeRequestDetailsDto>> GetChangeRequestDetails(Guid id)
        {
            var changeRequest = await _context.ChangeRequests.FindAsync(id);

            if (changeRequest == null)
            {
                return NotFound();
            }

            return _changeRequestsProvider.ConvertToDetailsView(changeRequest);
        }

        // PUT: api/ChangeRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChangeRequest(Guid id, WriteChangeRequestDto dto)
        {
            var changeRequest = _context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestId == id);
            if (changeRequest == null)
            {
                return NotFound();
            }

            var validationResult = _validator.IsValidChangeRequest(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            changeRequest = _mapper.Map(dto, changeRequest);
            _context.Entry(changeRequest).State = EntityState.Modified;

            try
            {
                _changeRequestsProvider.SaveChangeRequestTagMaps(dto, changeRequest.ChangeRequestId);
                _changeRequestsProvider.SaveChangeRequestGroupMaps(dto, changeRequest.ChangeRequestId);
                await _changeRequestsProvider.MergeApprovals(dto, changeRequest.ChangeRequestId);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_changeRequestsProvider.ChangeRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChangeRequests
        [HttpPost]
        public async Task<ActionResult<ChangeRequest>> PostChangeRequest(WriteChangeRequestDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == dto.UserId);
            if (user == null || user.UserProfile != UserProfile.ReadAndWriteOnly)
            {
                return BadRequest("Access Denied");
            }
            var validationResult = _validator.IsValidChangeRequest(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var changeRequest = _changeRequestsProvider.GetNewChangeRequest(dto);

            _context.ChangeRequests.Add(changeRequest);

            _changeRequestsProvider.SaveChangeRequestTagMaps(dto, changeRequest.ChangeRequestId);
            _changeRequestsProvider.SaveChangeRequestGroupMaps(dto, changeRequest.ChangeRequestId);
            await _changeRequestsProvider.MergeApprovals(dto, changeRequest.ChangeRequestId);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetChangeRequestDetails", new { id = changeRequest.ChangeRequestId }, _changeRequestsProvider.ConvertToDetailsView(changeRequest));
        }

        // DELETE: api/ChangeRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequest(Guid id)
        {
            var changeRequest = await _context.ChangeRequests.FindAsync(id);
            if (changeRequest == null || changeRequest.ChangeRequestStatus == ChangeRequestStatus.Abandoned)
            {
                return NotFound();
            }

            changeRequest.ChangeRequestStatus = ChangeRequestStatus.Abandoned;
            _context.Entry(changeRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_changeRequestsProvider.ChangeRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    }
}
