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

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public CommentsController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int changeRequestId)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set is null.");
            }
            return changeRequestId > 0 ? await _context.Comments.Where(x => x.ChangeRequestId == changeRequestId).ToListAsync()
                : await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentDetails(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set is null.");
            }
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return BadRequest();
            }

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
            if (_context.Comments == null)
            {
                return Problem("Entity set is null.");
            }

            var validationResult = _validator.IsValidComment(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var comment = _mapper.Map<Comment>(dto);
            comment.Created = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
