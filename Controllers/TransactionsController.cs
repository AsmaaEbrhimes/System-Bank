using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ContextApi _context;


        public TransactionsController(ContextApi context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTransacactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }


        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(int accountId, decimal amount)
        {
            if (amount <= 0) return BadRequest("المبلغ يجب أن يكون أكبر من الصفر.");
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return NotFound("الحساب غير موجود.");
            account.Balance += amount;

            var transaction = new Transaction
            {
                Account = account,
                Amount = amount,
                TransactionType = "Deposit",
                BalanceAfter = account.Balance,
                CreatedAt = DateTime.Now
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "تمت عملية الإيداع بنجاح.",
                currentBalance = account.Balance
            });

        }



        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0) return BadRequest("المبلغ يجب أن يكون أكبر من الصفر.");
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return NotFound("الحساب غير موجود.");
            account.Balance -= amount;

            if (amount > account.Balance) return BadRequest("رصيدك الحالي لا يكفي لإتمام العملية."); ;

            var transaction = new Transaction
            {
                Account = account,
                Amount = amount,
                TransactionType = "Deposit",
                BalanceAfter = account.Balance,
                CreatedAt = DateTime.Now
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "تمت عملية السحب بنجاح.",
                currentBalance = account.Balance
            });

        }




        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            var toAccount = await _context.Accounts.FindAsync(toAccountId);

            if (amount <= 0)
            {
                return BadRequest("المبلغ يجب أن يكون أكبر من الصفر.");
            }

            if (fromAccountId == toAccountId)
                return BadRequest("لا يمكن التحويل لنفس الحساب.");


            if (fromAccount == null || toAccount == null)
                return NotFound("أحد الحسابات أو كلاهما غير موجود.");

            if (fromAccount.Balance < amount)
                return BadRequest("رصيد الحساب المرسل لا يكفي للتحويل.");

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            // تسجيل العملية للحساب الساحب
            var transactionFrom = new Transaction
            {
                AccountId = fromAccountId,
                Amount = amount,
                TransactionType = "Transfer Out",
                BalanceAfter = fromAccount.Balance, // رصيد المرسل بعد الخصم
                CreatedAt = DateTime.Now
            };

            // تسجيل العملية للحساب المستلم
            var transactionTo = new Transaction
            {
                AccountId = toAccountId,
                Amount = amount,
                TransactionType = "Transfer In",
                BalanceAfter = toAccount.Balance, // رصيد المستلم بعد الإضافة
                CreatedAt = DateTime.Now
            };

            _context.Transactions.Add(transactionFrom);
            _context.Transactions.Add(transactionTo);

            await _context.SaveChangesAsync();

            return Ok(new { message = "تمت عملية التحويل بنجاح.", yourNewBalance = fromAccount.Balance });

        }

    }
}
