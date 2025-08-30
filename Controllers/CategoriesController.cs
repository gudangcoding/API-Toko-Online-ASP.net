using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Models;

namespace MyApiApp.Controllers
{
    /// <summary>
    /// Controller untuk mengelola operasi Category (kategori produk)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mendapatkan semua kategori yang aktif
        /// </summary>
        /// <returns>Daftar semua kategori aktif</returns>
        /// <response code="200">Berhasil mendapatkan daftar kategori</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>), 200)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Mendapatkan kategori berdasarkan ID beserta produk-produknya
        /// </summary>
        /// <param name="id">ID kategori</param>
        /// <returns>Data kategori beserta produk-produknya</returns>
        /// <response code="200">Berhasil mendapatkan data kategori</response>
        /// <response code="404">Kategori tidak ditemukan</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            return Ok(category);
        }

        /// <summary>
        /// Membuat kategori baru
        /// </summary>
        /// <param name="category">Data kategori yang akan dibuat</param>
        /// <returns>Data kategori yang berhasil dibuat</returns>
        /// <response code="201">Kategori berhasil dibuat</response>
        /// <response code="400">Data input tidak valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(Category), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                return BadRequest(new { message = "Category name is required" });
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        /// <summary>
        /// Update data kategori
        /// </summary>
        /// <param name="id">ID kategori</param>
        /// <param name="category">Data yang akan diupdate</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Berhasil mengupdate data kategori</response>
        /// <response code="400">Data input tidak valid</response>
        /// <response code="404">Kategori tidak ditemukan</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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
        /// Menghapus kategori (soft delete)
        /// </summary>
        /// <param name="id">ID kategori</param>
        /// <returns>Tidak ada konten</returns>
        /// <response code="204">Kategori berhasil dihapus</response>
        /// <response code="404">Kategori tidak ditemukan</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
