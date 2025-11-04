using System;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Budget;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetTracker.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly IAccountService _accountService;

        public BudgetsController(IBudgetService budgetService, IAccountService accountService)
        {
            _budgetService = budgetService;
            _accountService = accountService;
        }

        // ========== MVC VIEW METHODS ==========

        // GET: /Budgets
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            return View(budgets);
        }

        // GET: /Budgets/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _budgetService.GetBudgetByIdAsync(id.Value);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        // GET: /Budgets/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            ViewBag.Accounts = accounts;
            return View();
        }

        // POST: /Budgets/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBudgetDto createDto)
        {
            if (!createDto.IsValid(out string errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            if (ModelState.IsValid)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _budgetService.CreateBudgetAsync(createDto, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(userId);
            ViewBag.Accounts = accounts;
            return View(createDto);
        }

        // GET: /Budgets/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _budgetService.GetBudgetByIdAsync(id.Value);
            if (budget == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateBudgetDto
            {
                Name = budget.Name,
                BudgetAmount = budget.BudgetAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                AccountId = budget.AccountId
            };

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            ViewBag.Accounts = accounts;

            return View(updateDto);
        }

        // POST: /Budgets/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateBudgetDto updateDto)
        {
            if (!updateDto.IsValid(out string errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            if (ModelState.IsValid)
            {
                var updated = await _budgetService.UpdateBudgetAsync(id, updateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            ViewBag.Accounts = accounts;
            return View(updateDto);
        }

        // GET: /Budgets/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _budgetService.GetBudgetByIdAsync(id.Value);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        // POST: /Budgets/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _budgetService.DeleteBudgetAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ========== API METHODS ==========

        // 1. GET ALL: /api/Budgets
        [Route("api/[controller]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BudgetViewDto>>> GetBudgets()
        {
            var budgetDtos = await _budgetService.GetAllBudgetsAsync();
            return Ok(budgetDtos);
        }

        // 2. GET BY ID: /api/Budgets/5
        [Route("api/[controller]/{id}")]
        [HttpGet]
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
        [Route("api/[controller]/{id}")]
        [HttpPut]
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
        [Route("api/[controller]")]
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
        [Route("api/[controller]/{id}")]
        [HttpDelete]
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