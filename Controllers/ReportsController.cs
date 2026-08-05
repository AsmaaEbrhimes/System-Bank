using Banking.Model;
using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ContextApi _context;

        public ReportsController(ContextApi context)
        {
            _context = context;
        }

        // =======================================================
        // 1. توليد كشف حساب لفترة محددة (Bank Statement)
        // =======================================================
        [HttpGet("statement/{accountId}")]
        public async Task<IActionResult> GetAccountStatement(
            int accountId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return NotFound("الحساب البنكي غير موجود.");
            DateTime start = fromDate ?? DateTime.Now.AddDays(-30);
            DateTime end = toDate ?? DateTime.Now;
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.CreatedAt >= start && t.CreatedAt <= end)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            decimal totalDeposits = transactions.Where(t => t.TransactionType == "Deposit" || t.TransactionType == "إيداع").Sum(t => t.Amount);
            decimal totalWithdrawals = transactions.Where(t => t.TransactionType != "Deposit" || t.TransactionType != "إيداع").Sum(t => t.Amount);
            var statement = new AccountStatementDto
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                CurrentBalance = account.Balance,
                FromDate = start,
                ToDate = end,
                TotalDeposits = totalDeposits,
                TotalWithdrawals = totalWithdrawals,
                Transactions = transactions.Select(t => new TransactionDto
                {
                    TransactionId = t.Id,
                    Type = t.TransactionType,
                    Amount = t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Date = t.CreatedAt
                }).ToList()
            };

            return Ok(statement);
        }





        // =======================================================
        // 2. الملخص المالي الشامل للعميل (Customer Summary)
        // =======================================================

        [HttpGet("customer-summary/{customerId}")]
        public async Task<IActionResult> GetCustomerSummary(int customerId)
        {
            var customer =  await _context.GetCustomers.Include(c => c.Accounts).FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null) return NotFound("العميل غير موجود");
            var accountIds = customer.Accounts.Select(a => a.Id).ToList();

            // حساب بيانات الكروت
            int activeCardsCount = await _context.Cards
                .CountAsync(c => accountIds.Contains(c.AccountId) && !c.IsBlocked);

            var activeLoans = await _context.Loans
                .Where(l => accountIds.Contains(l.AccountId.Value) && l.Status == "Approved")
                .ToListAsync();

            var summary = new CustomerSummaryDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.FullName,
                TotalAccountsCount = customer.Accounts.Count,
                TotalBalanceAcrossAccounts = customer.Accounts.Sum(a => a.Balance),
                ActiveCardsCount = activeCardsCount,
                TotalActiveLoansAmount = activeLoans.Sum(l => l.Amount),
                TotalRemainingLoansAmount = activeLoans.Sum(l => l.RemainingAmount ?? 0)
            };

            return Ok(summary);
        }

    }
}
