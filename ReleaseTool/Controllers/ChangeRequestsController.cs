using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.ChangeRequests;
using ReleaseTool.Features.ChangeRequests.Models.Dtos;
using ReleaseTool.Features.Users;
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

            mapped.ForEach(x => x.UserDisplayName = _usersProvider.GetDisplayName(x.UserId));

            return mapped.OrderByDescending(x => x.Created).ToList();
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
                _changeRequestsProvider.MergeApprovals(dto, changeRequest.ChangeRequestId);

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
            var validationResult = _validator.IsValidChangeRequest(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var changeRequest = _changeRequestsProvider.GetNewChangeRequest(dto);

            _context.ChangeRequests.Add(changeRequest);

            _changeRequestsProvider.SaveChangeRequestTagMaps(dto, changeRequest.ChangeRequestId);
            _changeRequestsProvider.SaveChangeRequestGroupMaps(dto, changeRequest.ChangeRequestId);
            _changeRequestsProvider.MergeApprovals(dto, changeRequest.ChangeRequestId);

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
            _changeRequestsProvider.DeleteApprovals(id);

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
