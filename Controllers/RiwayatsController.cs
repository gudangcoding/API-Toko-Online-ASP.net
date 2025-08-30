using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Models;

namespace MyApiApp.Controllers
{
    /// <summary>
    /// Controller untuk mengelola operasi Riwayat (aktivitas user)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RiwayatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RiwayatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mendapatkan semua riwayat aktivitas
        /// </summary>
        /// <returns>Daftar semua riwayat aktivitas beserta data user</returns>
        /// <response code="200">Berhasil mendapatkan daftar riwayat</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Riwayat>), 200)]
        public async Task<ActionResult<IEnumerable<Riwayat>>> GetRiwayats()
        {
            var riwayats = await _context.Riwayats
                .Include(r => r.User)
                .OrderByDescending(r => r.ActivityDate)
                .ToListAsync();
            return Ok(riwayats);
        }

        /// <summary>
        /// Mendapatkan riwayat aktivitas berdasarkan user
        /// </summary>
        /// <param name="userId">ID user</param>
        /// <returns>Daftar riwayat aktivitas user tertentu</returns>
        /// <response code="200">Berhasil mendapatkan daftar riwayat</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Riwayat>), 200)]
        public async Task<ActionResult<IEnumerable<Riwayat>>> GetRiwayatsByUser(int userId)
        {
            var riwayats = await _context.Riwayats
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ActivityDate)
                .ToListAsync();
            return Ok(riwayats);
        }

        /// <summary>
        /// Mendapatkan riwayat berdasarkan ID
        /// </summary>
        /// <param name="id">ID riwayat</param>
        /// <returns>Data riwayat beserta data user</returns>
        /// <response code="200">Berhasil mendapatkan data riwayat</response>
        /// <response code="404">Riwayat tidak ditemukan</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Riwayat), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<Riwayat>> GetRiwayat(int id)
        {
            var riwayat = await _context.Riwayats
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (riwayat == null)
            {
                return NotFound(new { message = "Riwayat not found" });
            }

            return Ok(riwayat);
        }

        /// <summary>
        /// Membuat riwayat aktivitas baru
        /// </summary>
        /// <param name="riwayat">Data riwayat yang akan dibuat</param>
        /// <returns>Data riwayat yang berhasil dibuat</returns>
        /// <response code="201">Riwayat berhasil dibuat</response>
        /// <response code="400">Data input tidak valid atau user tidak ditemukan</response>
        [HttpPost]
        [ProducesResponseType(typeof(Riwayat), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult<Riwayat>> CreateRiwayat(Riwayat riwayat)
        {
            // Verify user exists
            var user = await _context.Users.FindAsync(riwayat.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            _context.Riwayats.Add(riwayat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRiwayat), new { id = riwayat.Id }, riwayat);
        }

        /// <summary>
        /// Mendapatkan daftar tipe aktivitas yang tersedia
        /// </summary>
        /// <returns>Daftar tipe aktivitas</returns>
        /// <response code="200">Berhasil mendapatkan daftar tipe aktivitas</response>
        [HttpGet("activity-types")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult<IEnumerable<object>> GetActivityTypes()
        {
            var activityTypes = Enum.GetValues(typeof(ActivityType))
                .Cast<ActivityType>()
                .Select(at => new { Value = (int)at, Name = at.ToString() })
                .ToList();

            return Ok(activityTypes);
        }

        /// <summary>
        /// Mendapatkan riwayat berdasarkan user dan tipe aktivitas
        /// </summary>
        /// <param name="userId">ID user</param>
        /// <param name="activityType">Tipe aktivitas</param>
        /// <returns>Daftar riwayat berdasarkan filter</returns>
        /// <response code="200">Berhasil mendapatkan daftar riwayat</response>
        [HttpGet("user/{userId}/activity/{activityType}")]
        [ProducesResponseType(typeof(IEnumerable<Riwayat>), 200)]
        public async Task<ActionResult<IEnumerable<Riwayat>>> GetRiwayatsByUserAndActivity(int userId, ActivityType activityType)
        {
            var riwayats = await _context.Riwayats
                .Include(r => r.User)
                .Where(r => r.UserId == userId && r.ActivityType == activityType)
                .OrderByDescending(r => r.ActivityDate)
                .ToListAsync();
            return Ok(riwayats);
        }

        /// <summary>
        /// Mendapatkan riwayat berdasarkan user dan rentang tanggal
        /// </summary>
        /// <param name="userId">ID user</param>
        /// <param name="startDate">Tanggal mulai</param>
        /// <param name="endDate">Tanggal akhir</param>
        /// <returns>Daftar riwayat dalam rentang tanggal tertentu</returns>
        /// <response code="200">Berhasil mendapatkan daftar riwayat</response>
        [HttpGet("user/{userId}/date-range")]
        [ProducesResponseType(typeof(IEnumerable<Riwayat>), 200)]
        public async Task<ActionResult<IEnumerable<Riwayat>>> GetRiwayatsByUserAndDateRange(
            int userId, 
            DateTime startDate, 
            DateTime endDate)
        {
            var riwayats = await _context.Riwayats
                .Include(r => r.User)
                .Where(r => r.UserId == userId && 
                           r.ActivityDate >= startDate && 
                           r.ActivityDate <= endDate)
                .OrderByDescending(r => r.ActivityDate)
                .ToListAsync();
            return Ok(riwayats);
        }
    }
}
