using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.DTOs;
using MyApiApp.Models;
using MyApiApp.Services;
using MyApiApp.Attributes;

namespace MyApiApp.Controllers
{
    /// <summary>
    /// Controller untuk mengelola operasi Order (pesanan)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] // Require authentication for all endpoints
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public OrdersController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Mendapatkan semua order
        /// </summary>
        /// <returns>Daftar semua order beserta detail user dan itemnya</returns>
        /// <response code="200">Berhasil mendapatkan daftar order</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), 200)]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDtos = orders.Select(MapToOrderResponseDto).ToList();
            return Ok(orderDtos);
        }

        /// <summary>
        /// Mendapatkan order berdasarkan ID
        /// </summary>
        /// <param name="id">ID order</param>
        /// <returns>Data order beserta detail user dan itemnya</returns>
        /// <response code="200">Berhasil mendapatkan data order</response>
        /// <response code="404">Order tidak ditemukan</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(MapToOrderResponseDto(order));
        }

        /// <summary>
        /// Mendapatkan order berdasarkan user yang sedang login
        /// </summary>
        /// <returns>Daftar order milik user yang sedang login</returns>
        /// <response code="200">Berhasil mendapatkan daftar order</response>
        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), 200)]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDtos = orders.Select(MapToOrderResponseDto).ToList();
            return Ok(orderDtos);
        }

        /// <summary>
        /// Mendapatkan order berdasarkan user ID (admin only)
        /// </summary>
        /// <param name="userId">ID user</param>
        /// <returns>Daftar order milik user tertentu</returns>
        /// <response code="200">Berhasil mendapatkan daftar order</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), 200)]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByUser(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDtos = orders.Select(MapToOrderResponseDto).ToList();
            return Ok(orderDtos);
        }

        /// <summary>
        /// Membuat order baru
        /// </summary>
        /// <param name="createOrderDto">Data order yang akan dibuat</param>
        /// <returns>Data order yang berhasil dibuat</returns>
        /// <response code="201">Order berhasil dibuat</response>
        /// <response code="400">Data input tidak valid atau stok tidak mencukupi</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!createOrderDto.OrderItems.Any())
            {
                return BadRequest(new { message = "Order must contain at least one item" });
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Generate order number
            var orderNumber = GenerateOrderNumber();

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = userId, // Use current user ID from token
                ShippingAddress = createOrderDto.ShippingAddress,
                Phone = createOrderDto.Phone,
                Notes = createOrderDto.Notes,
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0;

            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found" });
                }

                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for product {product.Name}" });
                }

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * itemDto.Quantity
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;

                // Update product stock
                product.Stock -= itemDto.Quantity;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload order with includes for response
            var createdOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToOrderResponseDto(createdOrder!));
        }

        /// <summary>
        /// Update status order
        /// </summary>
        /// <param name="id">ID order</param>
        /// <param name="updateStatusDto">Data status yang akan diupdate</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Berhasil mengupdate status order</response>
        /// <response code="404">Order tidak ditemukan</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            order.Status = updateStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateStatusDto.Notes))
            {
                order.Notes = updateStatusDto.Notes;
            }

            // Update dates based on status
            switch (updateStatusDto.Status)
            {
                case OrderStatus.Shipped:
                    order.ShippedDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredDate = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Membatalkan order
        /// </summary>
        /// <param name="id">ID order</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Order berhasil dibatalkan</response>
        /// <response code="400">Order tidak dapat dibatalkan</response>
        /// <response code="404">Order tidak ditemukan</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Check if user can cancel this order (only owner or admin)
            var userId = GetCurrentUserId();
            if (userId != order.UserId)
            {
                return Forbid();
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            {
                return BadRequest(new { message = "Cannot cancel order that is already being processed" });
            }

            // Restore product stock
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.Stock += orderItem.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return 0;
            }

            var token = authHeader.Substring("Bearer ".Length);
            var userId = _jwtService.GetUserIdFromToken(token);
            
            if (int.TryParse(userId, out int id))
            {
                return id;
            }

            return 0;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private static OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                UserName = order.User?.Name ?? "",
                ShippingAddress = order.ShippingAddress,
                Phone = order.Phone,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Notes = order.Notes,
                OrderDate = order.OrderDate,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}
