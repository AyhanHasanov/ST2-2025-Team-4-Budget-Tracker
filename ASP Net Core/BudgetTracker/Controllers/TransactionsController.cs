using System;
using System.Collections.Generic;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Transaction;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // 1. GET ALL: /api/Transactions
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewDto>>> GetTransactions()
        {
            var transactionDtos = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactionDtos);
        }

        // 2. GET BY ID: /api/Transactions/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionViewDto>> GetTransaction(int id)
        {
            var transactionDto = await _transactionService.GetTransactionByIdAsync(id);

            if (transactionDto == null)
            {
                return NotFound();
            }

            return Ok(transactionDto);
        }

        // 3. PUT: /api/Transactions/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutTransaction(int id, UpdateTransactionDto updateDto)
        {
            var updated = await _transactionService.UpdateTransactionAsync(id, updateDto);

            if (!updated)
            {
                return NotFound();
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
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var createdDto = await _transactionService.CreateTransactionAsync(createDto, currentUserId);

            if (createdDto == null)
            {
                return BadRequest("Account not found.");
            }

            return CreatedAtAction(nameof(GetTransaction), new { id = createdDto.Id }, createdDto);
        }

        // 5. DELETE: /api/Transactions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var deleted = await _transactionService.DeleteTransactionAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}