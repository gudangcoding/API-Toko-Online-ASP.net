using Microsoft.AspNetCore.Mvc;
using MyApiApp.DTOs;
using MyApiApp.Services;

namespace MyApiApp.Controllers
{
    /// <summary>
    /// Controller untuk mengelola operasi User (pengguna)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registrasi user baru
        /// </summary>
        /// <param name="createUserDto">Data user yang akan didaftarkan</param>
        /// <returns>Data user yang berhasil dibuat</returns>
        /// <response code="201">User berhasil dibuat</response>
        /// <response code="400">Data input tidak valid atau email sudah terdaftar</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponseDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult<UserResponseDto>> Register(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Login user dan mendapatkan JWT token
        /// </summary>
        /// <param name="loginDto">Kredensial login</param>
        /// <returns>JWT token dan data user</returns>
        /// <response code="200">Login berhasil</response>
        /// <response code="401">Email atau password salah</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(object), 401)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtService.GenerateToken(user);
            var expiresIn = 60; // minutes

            var response = new AuthResponseDto
            {
                Message = "Login successful",
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = expiresIn,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsActive = user.IsActive
                }
            };

            return Ok(response);
        }

        /// <summary>
        /// Validasi JWT token
        /// </summary>
        /// <returns>Informasi user dari token</returns>
        /// <response code="200">Token valid</response>
        /// <response code="401">Token tidak valid</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(TokenValidationDto), 200)]
        [ProducesResponseType(typeof(object), 401)]
        public async Task<ActionResult<TokenValidationDto>> GetCurrentUser()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token is required" });
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (!_jwtService.ValidateToken(token))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = _jwtService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var response = new TokenValidationDto
            {
                IsValid = true,
                UserId = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name
            };

            return Ok(response);
        }

        /// <summary>
        /// Mendapatkan semua user yang aktif
        /// </summary>
        /// <returns>Daftar semua user aktif</returns>
        /// <response code="200">Berhasil mendapatkan daftar user</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Mendapatkan user berdasarkan ID
        /// </summary>
        /// <param name="id">ID user</param>
        /// <returns>Data user</returns>
        /// <response code="200">Berhasil mendapatkan data user</response>
        /// <response code="404">User tidak ditemukan</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Update data user
        /// </summary>
        /// <param name="id">ID user</param>
        /// <param name="updateUserDto">Data yang akan diupdate</param>
        /// <returns>Data user yang telah diupdate</returns>
        /// <response code="200">Berhasil mengupdate data user</response>
        /// <response code="404">User tidak ditemukan</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Menghapus user (soft delete)
        /// </summary>
        /// <param name="id">ID user</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">User berhasil dihapus</response>
        /// <response code="404">User tidak ditemukan</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }

            return NoContent();
        }
    }
}
