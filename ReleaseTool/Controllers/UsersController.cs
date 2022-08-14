﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReleaseTool.Common;
using ReleaseTool.DataAccess;
using ReleaseTool.Features.Users.Models.DataAccess;
using ReleaseTool.Features.Users.Models.Dtos;
using XSystem.Security.Cryptography;

namespace ReleaseTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ReleaseToolContext _context;
        private readonly IMapper _mapper;
        private readonly IRuleValidator _validator;

        public UsersController(ReleaseToolContext context, IMapper mapper, IRuleValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetUsers(bool includeInactive)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();

            return includeInactive == true ? users.Select(x => _mapper.Map<ReadUserDto>(x)).ToList() 
                : users.Select(x => _mapper.Map<ReadUserDto>(x)).Where(x => x.UserStatus != UserStatus.Inactive).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadUserDto>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return _mapper.Map<ReadUserDto>(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, WriteUserDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            user = _mapper.Map<WriteUserDto, User>(dto, user);
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(WriteUserDto dto)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ReleaseToolContext.Users' is null.");
            }

            var validationResult = _validator.IsValidUser(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Message);
            }

            var newUser = _mapper.Map<User>(dto);
            
            newUser.UserStatus = UserStatus.Active;
            newUser.Created = DateTime.Now;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var viewUser = _mapper.Map<ReadUserDto>(newUser);

            return CreatedAtAction("GetUser", new { id = newUser.UserId }, viewUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.UserStatus == UserStatus.Inactive)
            {
                return NotFound();
            }

            user.UserStatus = UserStatus.Inactive;
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }            
    }
}
