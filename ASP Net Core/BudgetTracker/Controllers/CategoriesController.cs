using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Category;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetTracker.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ========== MVC VIEW METHODS ==========

        // GET: /Categories
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // All users can view all categories
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: /Categories/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: /Categories/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Show all categories for parent category selection
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: /Categories/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto createDto)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _categoryService.CreateCategoryAsync(createDto, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            
            // Show all categories for parent category selection
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View(createDto);
        }

        // GET: /Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateCategoryDto
            {
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId
            };

            // Show all categories for parent category selection
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            ViewBag.CurrentCategoryId = id.Value;

            return View(updateDto);
        }

        // POST: /Categories/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryDto updateDto)
        {
            if (ModelState.IsValid)
            {
                var updated = await _categoryService.UpdateCategoryAsync(id, updateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            
            // Show all categories for parent category selection
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            ViewBag.CurrentCategoryId = id;
            return View(updateDto);
        }

        // GET: /Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: /Categories/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ========== API METHODS ==========

        // 1. GET ALL: /api/Categories
        [Route("api/[controller]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryViewDto>>> GetCategories()
        {
            var categoryDtos = await _categoryService.GetAllCategoriesAsync();
            return Ok(categoryDtos);
        }

        // 2. GET BY ID: /api/Categories/5
        [Route("api/[controller]/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryViewDto>> GetCategory(int id)
        {
            var categoryDto = await _categoryService.GetCategoryByIdAsync(id);

            if (categoryDto == null)
            {
                return NotFound();
            }

            return Ok(categoryDto);
        }

        // 3. PUT: /api/Categories/5
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]/{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PutCategory(int id, UpdateCategoryDto updateDto)
        {
            var updated = await _categoryService.UpdateCategoryAsync(id, updateDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // 4. POST: /api/Categories
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryViewDto>> PostCategory(CreateCategoryDto createDto)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var createdDto = await _categoryService.CreateCategoryAsync(createDto, currentUserId);

            return CreatedAtAction(nameof(GetCategory), new { id = createdDto.Id }, createdDto);
        }

        // 5. DELETE: /api/Categories/5
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]/{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
