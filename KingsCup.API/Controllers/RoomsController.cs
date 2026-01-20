using KingsCup.API.Data;
using KingsCup.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingsCup.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserCode == request.UserCode);
            if (user == null) return Unauthorized("ไม่พบ User นี้ครับ");

            var roomCode = GenerateRoomCode();

            var newRoom = new Room
            {
                RoomCode = roomCode,
                Status = "WAITING"
            };

            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            var hostPlayer = new RoomPlayer
            {
                UserId = user.Id,
                UserCode = user.UserCode,
                Nickname = user.Nickname,
                RoomId = newRoom.Id,
                IsReady = false

            };

            _context.RoomPlayers.Add(hostPlayer);
            await _context.SaveChangesAsync();

            return Ok(new { RoomCode = roomCode, Message = "สร้างห้องเสร็จแล้ว" });
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserCode == request.UserCode);
            if (user == null) return Unauthorized("ไม่พบ User นี้ครับ");

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == request.RoomCode);
            if (room == null) return NotFound("ไม่เจอห้องนี้ครับ เช็ครหัสใหม่อีกที");

            var existingPlayer = await _context.RoomPlayers
                .FirstOrDefaultAsync(rp => rp.RoomId == room.Id && rp.UserId == user.Id);

            if (existingPlayer != null)
            {
                return Ok(new { Message = "กลับเข้าห้องเดิมสำเร็จ", RoomId = room.Id });
            }

            var newPlayer = new RoomPlayer
            {
                UserId = user.Id,
                UserCode = user.UserCode,
                Nickname = user.Nickname,
                RoomId = room.Id,
                IsReady = false
            };

            _context.RoomPlayers.Add(newPlayer);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "เข้าห้องสำเร็จ!", RoomId = room.Id });
        }

        [HttpGet("{roomCode}")]
        public async Task<IActionResult> GetRoom(string roomCode)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room == null) return NotFound("ไม่เจอห้องนี้");

            var players = await _context.RoomPlayers
                .Where(rp => rp.RoomId == room.Id)
                .OrderBy(rp => rp.Id)
                .ToListAsync();

            var lastCard = await _context.GameCards
                .Where(c => c.RoomId == room.Id && c.IsDrawn)
                .OrderByDescending(c => c.OrderIndex)
                .ToListAsync();

            var currentCard = lastCard.FirstOrDefault();

            return Ok(new { Room = room, Players = players, LastCard = currentCard });
        }

        [HttpPost("toggle-ready")]
        public async Task<IActionResult> ToggleReady([FromBody] ToggleReadyRequest request)
        {
            var players = await _context.RoomPlayers
                .FirstOrDefaultAsync(rp => rp.UserCode == request.UserCode && rp.RoomId == request.RoomId);
            if (players == null) return NotFound("คุณไม่ได้อยู่ในห้องนี้");

            players.IsReady = !players.IsReady;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "เปลี่ยนสถานะแล้ว", IsReady = players.IsReady });
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveRoom([FromBody] LeaveRoomRequest request)
        {
            var player = await _context.RoomPlayers
                .FirstOrDefaultAsync(rp => rp.UserCode == request.UserCode && rp.RoomId == request.RoomId);
            if(player != null)
            {
                _context.RoomPlayers.Remove(player);
                await _context.SaveChangesAsync();
            }
            return Ok(new { Message = "ออกจากห้องแล้ว" });
        }

        [HttpPost("start-game")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == request.RoomCode);

            string[] suits = { "Spades", "Hearts", "diamonds", "Clubs" };
            string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            var newDeck = new List<GameCard>();
            var random = new Random();

            foreach(var suit in suits)
            {
                foreach(var rank in ranks)
                {
                    newDeck.Add(new GameCard
                    {
                        RoomId = room.Id,
                        Suit = suit,
                        Rank = rank,
                        IsDrawn = false,
                        OrderIndex = random.Next()
                    });
                }
            }

            var oldCards = await _context.GameCards.Where(c => c.RoomId == room.Id).ToListAsync();
            _context.GameCards.RemoveRange(oldCards);

            await _context.GameCards.AddRangeAsync(newDeck);
            room.Status = "PLAYING";

            var firstPlayer = await _context.RoomPlayers
                .Where(p => p.RoomId == room.Id)
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync();
            if (firstPlayer != null)
            {
                room.CurrentTurnUserCode = firstPlayer.UserCode;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "เริ่มเกมแล้ว! สับไพ่เรียบร้อย" });
        }

        [HttpPost("draw-card")]
        public async Task<IActionResult> DrawCard([FromBody] DrawCardRequest request)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == request.RoomCode);
            if (room == null) return NotFound("ไม่พบห้อง");

            if (room.CurrentTurnUserCode != request.UserCode)
                return BadRequest("ยังไม่ใช่เทิร์นของคุณ!");

            var card = await _context.GameCards
                .Where(c => c.RoomId == room.Id && !c.IsDrawn)
                .OrderBy(c => c.OrderIndex)
                .FirstOrDefaultAsync();

            if (card == null) return BadRequest("ไพ่หมดสำหรับแล้วจ้า!");

            card.IsDrawn = true;
            card.DrawnByUserCode = request.UserCode;

            var players = await _context.RoomPlayers
                .Where(p => p.RoomId == room.Id)
                .OrderBy(p => p.Id)
                .ToListAsync();

            var currentIndex = players.FindIndex(p => p.UserCode == request.UserCode);
            var nextIndex = (currentIndex + 1) % players.Count;

            room.CurrentTurnUserCode = players[nextIndex].UserCode;
            await _context.SaveChangesAsync();
            return Ok(new { Card = card, NextTurn = room.CurrentTurnUserCode });
        }
    }

    public class CreateRoomRequest
    {
        public required string UserCode { get; set; }
    }

    public class JoinRoomRequest
    {
        public required string UserCode { get; set; }
        public required string RoomCode { get; set; }
    }

    public class ToggleReadyRequest
    {
        public required string UserCode { get; set; }
        public int RoomId { get; set; }
    }

    public class LeaveRoomRequest
    {
        public required string UserCode { get; set; }
        public int RoomId { get; set; }
    }

    public class StartGameRequest
    {
        public required string RoomCode { get; set; }
    }

    public class DrawCardRequest
    {
        public required string RoomCode { get; set; }
        public required string UserCode { get; set; }
    }
}
