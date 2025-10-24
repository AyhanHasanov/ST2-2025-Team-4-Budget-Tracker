using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Data;
using BudgetTracker.Models.DTOs.Budget;
using BudgetTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BudgetsController(AppDbContext context)
        {
            _context = context;
        }

        
        private BudgetViewDto MapToViewDto(Budget budget)
        {
            return new BudgetViewDto
            {
                Id = budget.Id,
                BudgetAmount = budget.BudgetAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CreatedAt = budget.CreatedAt,
                ModifiedAt = budget.ModifiedAt,

                
                SpentAmount = 0,
                IncomeAmount = 0,
                RemainingAmount = 0,
                InLimit = true,
                Exceeded = false
            };
        }

        // 1. GET ALL: /api/Budgets
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BudgetViewDto>>> GetBudgets()
        {
            var budgets = await _context.Budgets.ToListAsync();
            var budgetDtos = budgets.Select(b => MapToViewDto(b)).ToList();

            return Ok(budgetDtos);
        }

        // 2. GET BY ID: /api/Budgets/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BudgetViewDto>> GetBudget(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            var budgetDto = MapToViewDto(budget);
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

            var existingBudget = await _context.Budgets.FindAsync(id);

            if (existingBudget == null)
            {
                return NotFound();
            }

            existingBudget.BudgetAmount = updateDto.BudgetAmount;
            existingBudget.StartDate = updateDto.StartDate;
            existingBudget.EndDate = updateDto.EndDate;
            existingBudget.ModifiedAt = DateTime.UtcNow;

            _context.Entry(existingBudget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
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

            var budget = new Budget
            {
                BudgetAmount = createDto.BudgetAmount,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,

                UserId = currentUserId, 
                Currency = "BGN" 
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            var createdDto = MapToViewDto(budget);

            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, createdDto);
        }

        // 5. DELETE: /api/Budgets/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
}