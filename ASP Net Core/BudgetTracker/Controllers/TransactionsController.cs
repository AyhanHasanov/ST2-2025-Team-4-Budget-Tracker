using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Data;
using BudgetTracker.Models.DTOs.Transaction;
using BudgetTracker.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // Хелпър функция за мапиране на единична транзакция към DTO
        private TransactionViewDto MapToViewDto(Transaction transaction)
        {
            return new TransactionViewDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,

                AccountId = transaction.AccountId,
                AccountName = transaction.Account?.Name ?? string.Empty,

                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,

                BudgetId = transaction.BudgetId,

                CreatedAt = transaction.CreatedAt,
                ModifiedAt = transaction.ModifiedAt
            };
        }

        // ====================================================================
        // БИЗНЕС ЛОГИКА: Актуализиране на баланса на Account
        // ====================================================================

        
        private void UpdateAccountBalance(Account account, decimal amount, string type, bool isReversal = false)
        {
            decimal factor = (type == "Income") ? 1 : -1;
            if (isReversal)
            {
                factor *= -1; 
            }

            account.Balance += (amount * factor);
            _context.Entry(account).State = EntityState.Modified;
        }

        // 1. GET ALL: /api/Transactions
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewDto>>> GetTransactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .ToListAsync();

            var transactionDtos = transactions.Select(t => MapToViewDto(t)).ToList();

            return Ok(transactionDtos);
        }

        // 2. GET BY ID: /api/Transactions/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionViewDto>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            var transactionDto = MapToViewDto(transaction);
            return Ok(transactionDto);
        }

        // 3. PUT: /api/Transactions/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutTransaction(int id, UpdateTransactionDto updateDto)
        {
            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(existingTransaction.AccountId);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            // 1. Обръщаме старата транзакция
            UpdateAccountBalance(account, existingTransaction.Amount, existingTransaction.Type, isReversal: true);

            // 2. Актуализираме Transaction Entity
            existingTransaction.Amount = updateDto.Amount;
            existingTransaction.Currency = updateDto.Currency;
            existingTransaction.Date = updateDto.Date;
            existingTransaction.Description = updateDto.Description;
            existingTransaction.Type = updateDto.Type;
            
            existingTransaction.ModifiedAt = DateTime.UtcNow;

            _context.Entry(existingTransaction).State = EntityState.Modified;

            // 3. Прилагаме новата транзакция
            UpdateAccountBalance(account, existingTransaction.Amount, existingTransaction.Type, isReversal: false);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // 4. POST: /api/Transactions
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        public async Task<ActionResult<TransactionViewDto>> PostTransaction(CreateTransactionDto createDto)
        {
            // ЛОГИКА ЗА СИГУРНОСТ
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            // Намираме Account-а, за да актуализираме баланса му
            var account = await _context.Accounts.FindAsync(createDto.AccountId);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            
            var transaction = new Transaction
            {
                Amount = createDto.Amount,
                Currency = createDto.Currency,
                Date = createDto.Date,
                Description = createDto.Description,
                Type = createDto.Type,
                AccountId = createDto.AccountId,
                CategoryId = createDto.CategoryId,
                BudgetId = createDto.BudgetId,
                UserId = currentUserId 
            };

            _context.Transactions.Add(transaction);

            // 2. АКТУАЛИЗИРАНЕ НА БАЛАНСА НА СМЕТКАТА
            UpdateAccountBalance(account, transaction.Amount, transaction.Type, isReversal: false);

            await _context.SaveChangesAsync();

            // Извличаме новия модел с включените връзки за пълното DTO
            var createdTransaction = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (createdTransaction == null)
            {
                return BadRequest("Transaction created but could not be retrieved.");
            }

            var createdDto = MapToViewDto(createdTransaction);
            return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdDto);
        }

        // 5. DELETE: /api/Transactions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Намираме Account-а, за да обърнем баланса му
            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account != null)
            {
                // Обръщаме транзакцията (isReversal=true)
                UpdateAccountBalance(account, transaction.Amount, transaction.Type, isReversal: true);
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}