using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
using ReleaseTool.Features.Change_Requests.Models.DataAccess;
using ReleaseTool.Features.Change_Requests.Models.Dtos;
using ReleaseTool.Features.Tags.Models.DataAccess;
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

        public ChangeRequestsController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/ChangeRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadChangeRequestDto>>> GetChangeRequests(bool includeInactive)
        {
            if (_context.ChangeRequests == null)
            {
                return NotFound();
            }
            var changeRequests = await _context.ChangeRequests.ToListAsync();

            return includeInactive == true ? 
                changeRequests.Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList()
                : changeRequests.Where(x => x.ChangeRequestStatus != ChangeRequestStatus.Abandoned)
                .Select(x => ConvertToView(x)).ToList();
        }

        // GET: api/ChangeRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadChangeRequestDto>> GetChangeRequest(int id)
        {
            if (_context.ChangeRequests == null 
                || _context.ChangeRequestTags == null
                || _context.Tags == null)
            {
                return NotFound();
            }
            var changeRequest = await _context.ChangeRequests.FindAsync(id);

            if (changeRequest == null)
            {
                return NotFound();
            }

            return ConvertToView(changeRequest);
        }

        // PUT: api/ChangeRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChangeRequest(Guid id, WriteChangeRequestDto dto)
        {
            if (_context.ChangeRequests == null)
            {
                return Problem("Entity set is null.");
            }

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
                SaveChangeRequestTagMaps(dto, changeRequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChangeRequestExists(id))
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
            if (_context.ChangeRequests == null
                || _context.ChangeRequestTags == null
                || _context.Tags == null)
            {
                return Problem("Entity set is null.");
            }

            var validationResult = _validator.IsValidChangeRequest(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var changeRequest = GetNewChangeRequest(dto);

            _context.ChangeRequests.Add(changeRequest);
            
            SaveChangeRequestTagMaps(dto, changeRequest);
            SaveChangeRequestGroupMaps(dto, changeRequest);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetChangeRequest", new { id = changeRequest.ChangeRequestId }, ConvertToView(changeRequest));
        }

        // DELETE: api/ChangeRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequest(Guid id)
        {
            if (_context.ChangeRequests == null)
            {
                return NotFound();
            }
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
                if (!ChangeRequestExists(id))
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

        private bool ChangeRequestExists(Guid id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }

        private List<string> GetTagNames(Guid changeRequestId)
        {
            List<string> tagNames = new();
            if (_context.ChangeRequestTags == null || _context.Tags == null)
            {
                throw new Exception("Error mapping Tags to Change Request");
            }
            var tagMaps = _context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequestId).ToList();
            foreach (var tag in tagMaps)
            {
                var tagObject = _context.Tags.FirstOrDefault(x => x.TagId == tag.TagId);
                if (tagObject != null)
                {
                    tagNames.Add(tagObject.Name);
                }
            }
            return tagNames;
        }

        private List<string> GetGroupNames(Guid changeRequestId)
        {
            List<string> groupNames = new();
            if (_context.ChangeRequestGroups == null || _context.Groups == null)
            {
                throw new Exception("Error mapping Groups to Change Request");
            }
            var groupMaps = _context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequestId).ToList();
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

        private void SaveChangeRequestTagMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest)
        {
            _context.RemoveRange(_context.ChangeRequestTags.Where(x => x.ChangeRequestId == changeRequest.ChangeRequestId));
            foreach (var tagName in dto.Tags)
            {
                var tag = _context.Tags.FirstOrDefault(x => x.Name == tagName);
                if (tag != null)
                {
                    _context.ChangeRequestTags.Add(new ChangeRequestTag
                    {
                        ChangeRequestId = changeRequest.ChangeRequestId,
                        TagId = tag.TagId
                    });
                }
            }
        }

        private void SaveChangeRequestGroupMaps(WriteChangeRequestDto dto, ChangeRequest changeRequest)
        {
            _context.RemoveRange(_context.ChangeRequestGroups.Where(x => x.ChangeRequestId == changeRequest.ChangeRequestId));
            foreach (var groupName in dto.UserGroups)
            {
                var group = _context.Groups.FirstOrDefault(x => x.GroupName == groupName);
                if (group != null)
                {
                    _context.ChangeRequestGroups.Add(new ChangeRequestGroup
                    {
                        ChangeRequestId = changeRequest.ChangeRequestId,
                        GroupId = group.GroupId
                    });
                }
            }
        }

        private ReadChangeRequestDto ConvertToView(ChangeRequest changeRequest)
        {
            var result = _mapper.Map<ReadChangeRequestDto>(changeRequest);
            result.Tags = GetTagNames(result.ChangeRequestId);
            result.UserGroups = GetGroupNames(result.ChangeRequestId);
            return result;
        }

        private ChangeRequest GetNewChangeRequest(WriteChangeRequestDto dto)
        {
            var changeRequest = _mapper.Map<ChangeRequest>(dto);
            changeRequest.Created = DateTime.Now;
            changeRequest.ChangeRequestStatus = ChangeRequestStatus.Active;

            return changeRequest;
        }
    }
}
