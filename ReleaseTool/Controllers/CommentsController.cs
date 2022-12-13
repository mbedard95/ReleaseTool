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
using ReleaseTool.Features.Comments.Models.DataAccess;
using ReleaseTool.Features.Comments.Models.Dtos;
using ReleaseTool.Features.Users;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;
        private readonly IUsersProvider _usersProvider;

        public CommentsController(ReleaseToolContext context, IMapper mapper, 
            IRuleValidator validator, IUsersProvider provider)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _usersProvider = provider;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadCommentDto>>> GetComments(Guid? changeRequestId)
        {
            var comments = (changeRequestId != null) ? await _context.Comments.Where(x => x.ChangeRequestId == changeRequestId).ToListAsync()
                : await _context.Comments.ToListAsync();

            var dtos = new List<ReadCommentDto>();
            foreach (var comment in comments)
            {
                var dto = _mapper.Map<ReadCommentDto>(comment);
                dto.DisplayName = _usersProvider.GetUserAttributes(comment.UserId).DisplayName;
                dtos.Add(dto);
            }
            return dtos.OrderByDescending(x => x.Created).ToList();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadCommentDto>> GetCommentDetails(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<ReadCommentDto>(comment);
            dto.DisplayName = _usersProvider.GetUserAttributes(comment.UserId).DisplayName;
            
            return dto;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, WriteCommentDto dto)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            comment = _mapper.Map(dto, comment);
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(WriteCommentDto dto)
        {
            var validationResult = _validator.IsValidComment(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var comment = _mapper.Map<Comment>(dto);
            comment.Created = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommentDetails", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(Guid id)
        {
            return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
