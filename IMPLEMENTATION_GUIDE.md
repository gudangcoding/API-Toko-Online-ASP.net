# Panduan Implementasi Detail - API Toko Online

## 📝 Langkah 3: Implementasi Model Entities

### 3.1 User Model (`Models/User.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Riwayat> Riwayats { get; set; } = new List<Riwayat>();
    }
}
```

### 3.2 Category Model (`Models/Category.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
```

### 3.3 Product Model (`Models/Product.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        public int CategoryId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
```

### 3.4 Order Model (`Models/Order.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
```

### 3.5 OrderItem Model (`Models/OrderItem.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        // Navigation Properties
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
```

### 3.6 Riwayat Model (`Models/Riwayat.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.Models
{
    public class Riwayat
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public ActivityType ActivityType { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Details { get; set; } = string.Empty;
        
        public DateTime ActivityDate { get; set; }
        
        [Required]
        public string IpAddress { get; set; } = string.Empty;
        
        [Required]
        public string UserAgent { get; set; } = string.Empty;
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
    }

    public enum ActivityType
    {
        Login = 1,
        Logout = 2,
        Register = 3,
        CreateOrder = 4,
        UpdateProfile = 5,
        PasswordChange = 6
    }
}
```

## 📝 Langkah 4: Database Context

### ApplicationDbContext (`Data/ApplicationDbContext.cs`)

```csharp
using Microsoft.EntityFrameworkCore;
using MyApiApp.Models;

namespace MyApiApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Riwayat> Riwayats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Riwayat configuration
            modelBuilder.Entity<Riwayat>(entity =>
            {
                entity.Property(e => e.ActivityDate).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Riwayats)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
```

## 📝 Langkah 5: DTOs

### User DTOs (`DTOs/UserDto.cs`)

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyApiApp.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
    }

    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
```

### Auth DTOs (`DTOs/AuthDto.cs`)

```csharp
namespace MyApiApp.DTOs
{
    public class AuthResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public UserResponseDto User { get; set; } = new UserResponseDto();
    }

    public class TokenValidationDto
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
```

## 📝 Langkah 6: Services

### User Service Interface (`Services/IUserService.cs`)

```csharp
using MyApiApp.DTOs;
using MyApiApp.Models;

namespace MyApiApp.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> AuthenticateUserAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
```

### User Service Implementation (`Services/UserService.cs`)

```csharp
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.DTOs;
using MyApiApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyApiApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            // Hash password
            var hashedPassword = HashPassword(createUserDto.Password);

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = hashedPassword,
                Phone = createUserDto.Phone,
                Address = createUserDto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToUserResponseDto(user);
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null) return null;

            var hashedPassword = HashPassword(password);
            return user.Password == hashedPassword ? user : null;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            return user != null ? MapToUserResponseDto(user) : null;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            return users.Select(MapToUserResponseDto).ToList();
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!string.IsNullOrEmpty(updateUserDto.Name))
                user.Name = updateUserDto.Name;
            
            if (!string.IsNullOrEmpty(updateUserDto.Phone))
                user.Phone = updateUserDto.Phone;
            
            if (!string.IsNullOrEmpty(updateUserDto.Address))
                user.Address = updateUserDto.Address;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToUserResponseDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null)
                return false;

            var currentHashedPassword = HashPassword(currentPassword);
            if (user.Password != currentHashedPassword)
                return false;

            user.Password = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            };
        }
    }
}
```

## 📝 Langkah 7: Custom Authorization

### Authorize Attribute (`Attributes/AuthorizeAttribute.cs`)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApiApp.Services;

namespace MyApiApp.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            if (jwtService == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is required" });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (!jwtService.ValidateToken(token))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid token" });
                return;
            }
        }
    }
}
```

## 📝 Langkah 8: Controllers

### Users Controller (`Controllers/UsersController.cs`)

```csharp
using Microsoft.AspNetCore.Mvc;
using MyApiApp.DTOs;
using MyApiApp.Services;
using MyApiApp.Attributes;

namespace MyApiApp.Controllers
{
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

        [HttpGet("me")]
        [Authorize]
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

        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

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

        [HttpPut("{id}")]
        [Authorize]
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

        [HttpDelete("{id}")]
        [Authorize]
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
```

## 📝 Langkah 9: Program.cs Configuration

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyApiApp.Data;
using MyApiApp.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Toko Online",
        Version = "v1",
        Description = "API untuk sistem toko online dengan fitur User, Kategori, Produk, Order, dan Riwayat aktivitas",
        Contact = new OpenApiContact
        {
            Name = "Developer",
            Email = "developer@example.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add security definitions
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Toko Online v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root URL
        c.DocumentTitle = "API Toko Online - Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
```

## 📝 Langkah 10: Configuration Files

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyApiAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "MyApiApp",
    "Audience": "MyApiAppUsers",
    "ExpirationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### MyApiApp.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.5" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.5" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.0" />
  </ItemGroup>

</Project>
```

## 🚀 Build dan Run

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

## 🔐 Testing JWT Authentication

### 1. Register User
```http
POST /api/users/register
Content-Type: application/json

{
  "name": "Test User",
  "email": "test@example.com",
  "password": "password123",
  "phone": "081234567890",
  "address": "Jl. Test No. 123"
}
```

### 2. Login dan Dapatkan Token
```http
POST /api/users/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

### 3. Gunakan Token
```http
GET /api/users/me
Authorization: Bearer {your_jwt_token_here}
```

## 📚 Next Steps

Setelah implementasi dasar selesai, Anda dapat menambahkan:

1. **Category Controller** - CRUD operations untuk kategori
2. **Product Controller** - CRUD operations untuk produk
3. **Order Controller** - Order management dengan JWT protection
4. **Riwayat Controller** - Activity logging
5. **Additional Services** - Business logic untuk setiap domain
6. **Validation** - Custom validation attributes
7. **Error Handling** - Global exception handling
8. **Logging** - Structured logging
9. **Testing** - Unit tests dan integration tests
10. **Documentation** - API documentation yang lengkap

---

**Selamat! Anda telah berhasil membuat API toko online dengan JWT authentication! 🎉**
