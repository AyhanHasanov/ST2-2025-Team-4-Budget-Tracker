using BudgetTracker.Data;
using BudgetTracker.Models.Entities;
using BudgetTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllWithDetailsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Include(t => t.Budget)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Include(t => t.Budget)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllByAccountIdAsync(int accountId)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .Include(t => t.Category)
                .Include(t => t.Budget)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllByCategoryIdAsync(int categoryId)
        {
            return await _context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .Include(t => t.Account)
                .Include(t => t.Budget)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllByBudgetIdAsync(int budgetId)
        {
            return await _context.Transactions
                .Where(t => t.BudgetId == budgetId)
                .Include(t => t.Account)
                .Include(t => t.Category)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Include(t => t.Budget)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

