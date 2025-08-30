using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Models;

namespace MyApiApp.Controllers
{
    /// <summary>
    /// Controller untuk mengelola operasi Product (produk)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mendapatkan semua produk yang aktif
        /// </summary>
        /// <returns>Daftar semua produk aktif beserta kategorinya</returns>
        /// <response code="200">Berhasil mendapatkan daftar produk</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Mendapatkan produk berdasarkan ID
        /// </summary>
        /// <param name="id">ID produk</param>
        /// <returns>Data produk beserta kategorinya</returns>
        /// <response code="200">Berhasil mendapatkan data produk</response>
        /// <response code="404">Produk tidak ditemukan</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Mendapatkan produk berdasarkan kategori
        /// </summary>
        /// <param name="categoryId">ID kategori</param>
        /// <returns>Daftar produk dalam kategori tertentu</returns>
        /// <response code="200">Berhasil mendapatkan daftar produk</response>
        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// Membuat produk baru
        /// </summary>
        /// <param name="product">Data produk yang akan dibuat</param>
        /// <returns>Data produk yang berhasil dibuat</returns>
        /// <response code="201">Produk berhasil dibuat</response>
        /// <response code="400">Data input tidak valid atau kategori tidak ditemukan</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                return BadRequest(new { message = "Product name is required" });
            }

            if (product.Price < 0)
            {
                return BadRequest(new { message = "Price cannot be negative" });
            }

            if (product.Stock < 0)
            {
                return BadRequest(new { message = "Stock cannot be negative" });
            }

            // Verify category exists
            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "Category not found" });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// Update data produk
        /// </summary>
        /// <param name="id">ID produk</param>
        /// <param name="product">Data yang akan diupdate</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Berhasil mengupdate data produk</response>
        /// <response code="400">Data input tidak valid</response>
        /// <response code="404">Produk tidak ditemukan</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        /// <summary>
        /// Menghapus produk (soft delete)
        /// </summary>
        /// <param name="id">ID produk</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Produk berhasil dihapus</response>
        /// <response code="404">Produk tidak ditemukan</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
