using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Core.Model;
using UserManagementApi.Data;

namespace UserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Context _context;

        public UsersController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery]int page = 1, [FromQuery]int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            if(!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search));
            }

            var totalUsers = await query.CountAsync();
            var users = await query.Skip((page-1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                CurrentPage = page,
                pageSize = pageSize,
                Users = users
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User UpdatedUser)
        {
            if (id != UpdatedUser.Id) return BadRequest("User id mismatch");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = UpdatedUser.FirstName;
            user.LastName = UpdatedUser.LastName;
            user.Email = UpdatedUser.Email;
            user.DateOfBirth = UpdatedUser.DateOfBirth;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
    }
}
