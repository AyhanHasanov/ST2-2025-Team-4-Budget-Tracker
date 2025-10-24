using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Data;
using BudgetTracker.Models.DTOs.Account;
using BudgetTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET ALL: /api/Accounts
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountViewDto>>> GetAccounts()
        {
            var accounts = await _context.Accounts.ToListAsync();

            var accountDtos = accounts.Select(a => new AccountViewDto
            {
                Id = a.Id,
                Name = a.Name,
                Currency = a.Currency,
                Balance = a.Balance,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                ModifiedAt = a.ModifiedAt
            }).ToList();

            return Ok(accountDtos);
        }

        // 2. GET BY ID: /api/Accounts/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountViewDto>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            var accountDto = new AccountViewDto
            {
                Id = account.Id,
                Name = account.Name,
                Currency = account.Currency,
                Balance = account.Balance,
                Description = account.Description,
                CreatedAt = account.CreatedAt,
                ModifiedAt = account.ModifiedAt
            };

            return Ok(accountDto);
        }

        // 3. PUT: /api/Accounts/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAccount(int id, UpdateAccountDto updateDto)
        {
            var existingAccount = await _context.Accounts.FindAsync(id);

            if (existingAccount == null)
            {
                return NotFound();
            }

            existingAccount.Name = updateDto.Name;
            existingAccount.Currency = updateDto.Currency;
            
            existingAccount.Balance = updateDto.Balance;
            existingAccount.Description = updateDto.Description;
            existingAccount.ModifiedAt = DateTime.UtcNow;

            _context.Entry(existingAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // 4. POST: /api/Accounts
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

            var account = new Account
            {
                Name = createDto.Name,
                Currency = createDto.Currency,
                Balance = createDto.Balance,
                Description = createDto.Description,
                UserId = currentUserId 
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var createdDto = new AccountViewDto
            {
                Id = account.Id,
                Name = account.Name,
                Currency = account.Currency,
                Balance = account.Balance,
                Description = account.Description,
                CreatedAt = account.CreatedAt,
                ModifiedAt = account.ModifiedAt
            };

            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, createdDto);
        }

        // 5. DELETE: /api/Accounts/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}