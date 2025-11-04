using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using BudgetTracker.Models.DTOs.Transaction;
using BudgetTracker.Models.DTOs.Llm;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BudgetTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IBudgetService _budgetService;
        private readonly ILlmService _llmService;

        public TransactionsController(
            ITransactionService transactionService,
            IAccountService accountService,
            ICategoryService categoryService,
            IBudgetService budgetService,
            ILlmService llmService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            _budgetService = budgetService;
            _llmService = llmService;
        }

        // ========== MVC VIEW METHODS ==========

        // GET: /Transactions
        [Authorize]
        public async Task<IActionResult> Index(int? accountId)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Get all user's accounts for the filter dropdown
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            ViewBag.Accounts = accounts;
            ViewBag.SelectedAccountId = accountId;
            
            // Get transactions - filtered by account if specified
            IEnumerable<TransactionViewDto> transactions;
            if (accountId.HasValue)
            {
                transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId.Value);
                
                // Get the selected account for currency display
                var selectedAccount = accounts.FirstOrDefault(a => a.Id == accountId.Value);
                ViewBag.Currency = selectedAccount?.Currency ?? "BGN";
            }
            else
            {
                transactions = await _transactionService.GetTransactionsByUserIdAsync(currentUserId);
                ViewBag.Currency = null; // Mixed currencies
            }
            
            return View(transactions);
        }

        // GET: /Transactions/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: /Transactions/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            var categories = await _categoryService.GetAllCategoriesAsync(); // All users can see all categories
            var budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            
            ViewBag.Accounts = accounts;
            ViewBag.Categories = categories;
            ViewBag.Budgets = budgets;
            
            return View();
        }

        // POST: /Transactions/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTransactionDto createDto)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var createdDto = await _transactionService.CreateTransactionAsync(createDto, currentUserId);
                
                if (createdDto == null)
                {
                    ModelState.AddModelError(string.Empty, "Account not found.");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Accounts = await _accountService.GetAccountsByUserIdAsync(userId);
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(); // All users can see all categories
            ViewBag.Budgets = await _budgetService.GetBudgetsByUserIdAsync(userId);
            return View(createDto);
        }

        // GET: /Transactions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            if (transaction == null)
            {
                return NotFound();
            }

            var updateDto = new UpdateTransactionDto
            {
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                Type = transaction.Type,
                AccountId = transaction.AccountId,
                CategoryId = transaction.CategoryId,
                BudgetId = transaction.BudgetId
            };

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            var categories = await _categoryService.GetAllCategoriesAsync(); // All users can see all categories
            var budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            
            ViewBag.Accounts = accounts;
            ViewBag.Categories = categories;
            ViewBag.Budgets = budgets;

            return View(updateDto);
        }

        // POST: /Transactions/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTransactionDto updateDto)
        {
            if (ModelState.IsValid)
            {
                var updated = await _transactionService.UpdateTransactionAsync(id, updateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Accounts = await _accountService.GetAccountsByUserIdAsync(currentUserId);
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(); // All users can see all categories
            ViewBag.Budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            return View(updateDto);
        }

        // GET: /Transactions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id.Value);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: /Transactions/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ========== API METHODS ==========

        // 1. GET ALL: /api/Transactions
        [Route("api/[controller]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionViewDto>>> GetTransactions()
        {
            var transactionDtos = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactionDtos);
        }

        // 2. GET BY ID: /api/Transactions/5
        [Route("api/[controller]/{id}")]
        [HttpGet]
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
        [Route("api/[controller]/{id}")]
        [HttpPut]
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
        [Route("api/[controller]")]
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
        [Route("api/[controller]/{id}")]
        [HttpDelete]
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

        // ========== LLM INTEGRATION METHODS ==========

        // GET: /Transactions/GetSummary?accountId=1
        [Authorize]
        public async Task<IActionResult> GetSummary(int? accountId)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!accountId.HasValue)
            {
                TempData["Error"] = "Please select a specific account to get a summary.";
                return RedirectToAction(nameof(Index));
            }

            // Get transactions for the account
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId.Value);
            var expenseTransactions = transactions.Where(t => t.Type == "Expense").ToList();

            if (!expenseTransactions.Any())
            {
                TempData["Error"] = "No expense transactions found to summarize.";
                return RedirectToAction(nameof(Index), new { accountId });
            }

            // Get account and budget info
            var account = await _accountService.GetAccountByIdAsync(accountId.Value);
            if (account == null)
            {
                return NotFound();
            }

            // Get the user's budgets for this account
            var budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            var accountBudget = budgets.FirstOrDefault(b => b.AccountId == accountId.Value);
            double budgetAmount = accountBudget?.BudgetAmount != null ? (double)accountBudget.BudgetAmount : 1000.0; // Default budget if none set

            // Prepare expense items for LLM
            var expenseItems = expenseTransactions.Select(t => new ExpenseItemDto
            {
                Category = t.CategoryName ?? "Uncategorized",
                Amount = (double)t.Amount
            }).ToList();

            // Call LLM service
            var summaryRequest = new SummaryRequestDto
            {
                Expenses = expenseItems,
                Budget = (double)budgetAmount
            };

            var summaryResponse = await _llmService.GetExpenseSummaryAsync(summaryRequest);

            if (summaryResponse == null)
            {
                TempData["Error"] = "Failed to generate summary. Please ensure the LLM service is running on http://localhost:8000";
                return RedirectToAction(nameof(Index), new { accountId });
            }

            ViewBag.Summary = summaryResponse.Summary;
            ViewBag.TotalAmount = summaryResponse.TotalAmount;
            ViewBag.ExpenseCount = summaryResponse.ExpenseCount;
            ViewBag.AccountName = account.Name;
            ViewBag.Currency = account.Currency;
            ViewBag.Budget = budgetAmount;
            ViewBag.AccountId = accountId.Value;

            return View(expenseTransactions);
        }

        // GET: /Transactions/GetAdvice?accountId=1
        [Authorize]
        public async Task<IActionResult> GetAdvice(int? accountId)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!accountId.HasValue)
            {
                TempData["Error"] = "Please select a specific account to get advice.";
                return RedirectToAction(nameof(Index));
            }

            // Get account info
            var account = await _accountService.GetAccountByIdAsync(accountId.Value);
            if (account == null)
            {
                return NotFound();
            }

            ViewBag.AccountId = accountId.Value;
            ViewBag.AccountName = account.Name;
            ViewBag.Currency = account.Currency;

            return View();
        }

        // POST: /Transactions/GetAdvice
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAdvice(int accountId, string question)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(question))
            {
                TempData["Error"] = "Please enter a question.";
                return RedirectToAction(nameof(GetAdvice), new { accountId });
            }

            // Get transactions for the account
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            var expenseTransactions = transactions.Where(t => t.Type == "Expense").ToList();

            if (!expenseTransactions.Any())
            {
                TempData["Error"] = "No expense transactions found to analyze.";
                return RedirectToAction(nameof(GetAdvice), new { accountId });
            }

            // Get account and budget info
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

            var budgets = await _budgetService.GetBudgetsByUserIdAsync(currentUserId);
            var accountBudget = budgets.FirstOrDefault(b => b.AccountId == accountId);
            double budgetAmount = accountBudget?.BudgetAmount != null ? (double)accountBudget.BudgetAmount : 1000.0;

            // Prepare expense items for LLM
            var expenseItems = expenseTransactions.Select(t => new ExpenseItemDto
            {
                Category = t.CategoryName ?? "Uncategorized",
                Amount = (double)t.Amount
            }).ToList();

            // Call LLM service
            var adviceRequest = new AdviceRequestDto
            {
                Question = question,
                Expenses = expenseItems,
                Budget = (double)budgetAmount
            };

            var adviceResponse = await _llmService.GetBudgetingAdviceAsync(adviceRequest);

            if (adviceResponse == null)
            {
                TempData["Error"] = "Failed to generate advice. Please ensure the LLM service is running on http://localhost:8000";
                return RedirectToAction(nameof(GetAdvice), new { accountId });
            }

            ViewBag.AccountId = accountId;
            ViewBag.AccountName = account.Name;
            ViewBag.Currency = account.Currency;
            ViewBag.Advice = adviceResponse.Advice;
            ViewBag.Question = question;

            return View();
        }
    }
}
