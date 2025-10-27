using System;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Budget;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        // 1. GET ALL: /api/Budgets
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BudgetViewDto>>> GetBudgets()
        {
            var budgetDtos = await _budgetService.GetAllBudgetsAsync();
            return Ok(budgetDtos);
        }

        // 2. GET BY ID: /api/Budgets/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BudgetViewDto>> GetBudget(int id)
        {
            var budgetDto = await _budgetService.GetBudgetByIdAsync(id);

            if (budgetDto == null)
            {
                return NotFound();
            }

            return Ok(budgetDto);
        }

        // 3. PUT: /api/Budgets/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutBudget(int id, UpdateBudgetDto updateDto)
        {
            if (!updateDto.IsValid(out string errorMessage))
            {
                return BadRequest(new { error = errorMessage });
            }

            var updated = await _budgetService.UpdateBudgetAsync(id, updateDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // 4. POST: /api/Budgets
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        public async Task<ActionResult<BudgetViewDto>> PostBudget(CreateBudgetDto createDto)
        {
            if (!createDto.IsValid(out string errorMessage))
            {
                return BadRequest(new { error = errorMessage });
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var createdDto = await _budgetService.CreateBudgetAsync(createDto, currentUserId);

            return CreatedAtAction(nameof(GetBudget), new { id = createdDto.Id }, createdDto);
        }

        // 5. DELETE: /api/Budgets/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var deleted = await _budgetService.DeleteBudgetAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}