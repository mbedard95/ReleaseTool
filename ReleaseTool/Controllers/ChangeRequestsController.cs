using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.ChangeRequests;
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

        public ChangeRequestsController(
            ReleaseToolContext context, 
            IMapper mapper, 
            IRuleValidator validator,
            IChangeRequestsProvider changeRequestProvider)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _changeRequestsProvider = changeRequestProvider;
        }

        // GET: api/ChangeRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadChangeRequestDto>>> GetChangeRequests(bool includeInactive)
        {
            var changeRequests = await _context.ChangeRequests.ToListAsync();

            return includeInactive == true ? 
                changeRequests.Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList()
                : changeRequests.Where(x => x.ChangeRequestStatus != ChangeRequestStatus.Abandoned)
                .Select(x => _changeRequestsProvider.ConvertToView(x)).ToList();
        }

        // GET: api/ChangeRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadChangeRequestDto>> GetChangeRequest(int id)
        {
            var changeRequest = await _context.ChangeRequests.FindAsync(id);

            if (changeRequest == null)
            {
                return NotFound();
            }

            return _changeRequestsProvider.ConvertToView(changeRequest);
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
                _changeRequestsProvider.SaveChangeRequestTagMaps(dto, changeRequest);
                _changeRequestsProvider.SaveChangeRequestGroupMaps(dto, changeRequest);

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

            _changeRequestsProvider.SaveChangeRequestTagMaps(dto, changeRequest);
            _changeRequestsProvider.SaveChangeRequestGroupMaps(dto, changeRequest);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetChangeRequest", new { id = changeRequest.ChangeRequestId }, _changeRequestsProvider.ConvertToView(changeRequest));
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
