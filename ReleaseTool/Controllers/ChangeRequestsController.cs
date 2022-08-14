using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Change_Requests.Models;
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
        public async Task<ActionResult<IEnumerable<ReadChangeRequestDto>>> GetChangeRequest(bool includeInactive)
        {
            if (_context.ChangeRequests == null)
            {
                return NotFound();
            }
            var changeRequests = await _context.ChangeRequests.ToListAsync();

            var dtos = includeInactive == true ? 
                changeRequests.Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList()
                : changeRequests.Where(x => x.ChangeRequestStatus != ChangeRequestStatus.Abandoned)
                .Select(x => _mapper.Map<ReadChangeRequestDto>(x)).ToList();
            
            foreach (var dto in dtos)
            {
                dto.Tags = GetTagNames(dto.ChangeRequestId);
            }
            return dtos;
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

            var dto = _mapper.Map<ReadChangeRequestDto>(changeRequest);
            dto.Tags = GetTagNames(dto.ChangeRequestId);

            return dto;
        }

        // PUT: api/ChangeRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChangeRequest(int id, ChangeRequest changeRequest)
        {
            if (id != changeRequest.ChangeRequestId)
            {
                return BadRequest();
            }

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

        // POST: api/ChangeRequests
        [HttpPost]
        public async Task<ActionResult<ChangeRequest>> PostChangeRequest(WriteChangeRequestDto dto)
        {
            if (_context.ChangeRequests == null
                || _context.ChangeRequestTags == null
                || _context.Tags == null)
            {
                return Problem("Entity set 'ReleaseToolContext.ChangeRequest'  is null.");
            }

            var validationResult = _validator.IsValidChangeRequest(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var changeRequest = _mapper.Map<ChangeRequest>(dto);
            changeRequest.Created = DateTime.Now;
            changeRequest.ChangeRequestStatus = ChangeRequestStatus.Active;

            _context.ChangeRequests.Add(changeRequest);
            await _context.SaveChangesAsync();

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
            await _context.SaveChangesAsync();

            var viewChangeRequest = _mapper.Map<ReadChangeRequestDto>(changeRequest);
            viewChangeRequest.Tags = GetTagNames(viewChangeRequest.ChangeRequestId);

            return CreatedAtAction("GetChangeRequest", new { id = changeRequest.ChangeRequestId }, viewChangeRequest);
        }

        // DELETE: api/ChangeRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChangeRequest(int id)
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

        private bool ChangeRequestExists(int id)
        {
            return (_context.ChangeRequests?.Any(e => e.ChangeRequestId == id)).GetValueOrDefault();
        }

        private List<string> GetTagNames(int changeRequestId)
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
    }
}
