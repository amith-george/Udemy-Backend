using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyBackend.Data;
using UdemyBackend.DTOs.Category;
using UdemyBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection of your DbContext
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        // Used for basic dropdowns or lists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/with-subcategories
        // The heavy lifter for the Angular Frontend Navigation Bar
        [HttpGet("with-subcategories")]
        public async Task<ActionResult<IEnumerable<CategoryWithSubcategoriesDto>>> GetCategoriesWithSubcategories()
        {
            var categoriesTree = await _context.Categories
                .Include(c => c.Subcategories) // Eager load the relationship
                .Select(c => new CategoryWithSubcategoriesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Subcategories = c.Subcategories.Select(s => new SubcategoryDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CategoryId = s.CategoryId
                    }).ToList()
                })
                .ToListAsync();

            return Ok(categoriesTree);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            return Ok(category);
        }

        // POST: api/categories
        // Used by Admins/Instructors to create a new category
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Return the newly created category using the Read DTO
            var createdDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, createdDto);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryCreateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}