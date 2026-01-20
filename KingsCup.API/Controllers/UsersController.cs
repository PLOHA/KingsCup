using KingsCup.API.Data;
using KingsCup.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingsCup.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var count = await _context.Users.CountAsync();
            var nextId = count + 1;
            var newCode = $"P{nextId:D2}";

            var newUser = new User
            {
                UserCode = newCode,
                Nickname = request.Nickname,
                PhoneNumber = request.PhoneNumber
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { UserCode = newCode, Message = "สมัครสำเร็จ!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserCode == request.UserCode);

            if (user == null) { return Unauthorized("ไม่พบรหัสผู้เล่นคนนี้"); }
            return Ok(new { Message = $"ยินดีต้อนรับ คุณ{user.Nickname}!", User = user});
        }
    }
        public class RegisterRequest
        {
            public required string Nickname { get; set; }
            public required string PhoneNumber { get; set; }
        }

        public class LoginRequest
        {
            public required string UserCode { get; set; }
        }
}
