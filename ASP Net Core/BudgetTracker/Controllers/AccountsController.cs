using System;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Account;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetTracker.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // ========== MVC VIEW METHODS ==========

        // GET: /Accounts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            return View(accounts);
        }

        // GET: /Accounts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: /Accounts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Accounts/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountDto createDto)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _accountService.CreateAccountAsync(createDto, currentUserId);
                return RedirectToAction(nameof(Index));
            }
            return View(createDto);
        }

        // GET: /Accounts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateAccountDto
            {
                Name = account.Name,
                Currency = account.Currency,
                Balance = account.Balance,
                Description = account.Description
            };

            return View(updateDto);
        }

        // POST: /Accounts/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAccountDto updateDto)
        {
            if (ModelState.IsValid)
            {
                var updated = await _accountService.UpdateAccountAsync(id, updateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(updateDto);
        }

        // GET: /Accounts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: /Accounts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ========== API METHODS ==========

        // 1. GET ALL: /api/Accounts
        [Route("api/[controller]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountViewDto>>> GetAccounts()
        {
            var accountDtos = await _accountService.GetAllAccountsAsync();
            return Ok(accountDtos);
        }

        // 2. GET BY ID: /api/Accounts/5
        [Route("api/[controller]/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountViewDto>> GetAccount(int id)
        {
            var accountDto = await _accountService.GetAccountByIdAsync(id);

            if (accountDto == null)
            {
                return NotFound();
            }

            return Ok(accountDto);
        }

        // 3. PUT: /api/Accounts/5
        [Route("api/[controller]/{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAccount(int id, UpdateAccountDto updateDto)
        {
            var updated = await _accountService.UpdateAccountAsync(id, updateDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // 4. POST: /api/Accounts
        [Route("api/[controller]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        public async Task<ActionResult<AccountViewDto>> PostAccount(CreateAccountDto createDto)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var createdDto = await _accountService.CreateAccountAsync(createDto, currentUserId);

            return CreatedAtAction(nameof(GetAccount), new { id = createdDto.Id }, createdDto);
        }

        // 5. DELETE: /api/Accounts/5
        [Route("api/[controller]/{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var deleted = await _accountService.DeleteAccountAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}