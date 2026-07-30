using Banking.Model;
using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ContextApi _contextApi;

        public LoansController(ContextApi context)
        {
            _contextApi = context;
        }


        // =======================================================
        // 👤 1. الجزء الخاص بالعميل (CUSTOMER ENDPOINTS)
        // =======================================================

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLoan([FromBody] LoanApplyDto dto)
        {
            if (dto == null) return BadRequest("يرجى إدخال مبلغ صحيح للقرض.");

            var account = await _contextApi.Accounts.FindAsync(dto.AccountId);
            if (account == null) return BadRequest("الحساب البنكي غير موجود.");
            var loan = new Loan
            {
                Amount = dto.Amount,
                AccountId = dto.AccountId,
                Status = "Pending",
                ApplicationDate = DateTime.Now
            };

            _contextApi.Loans.Add(loan);
            await _contextApi.SaveChangesAsync();

            return Ok(new
            {
                message = "تم تقديم طلب القرض بنجاح وهو قيد مراجعة الموظف.",
                loanId = loan.Id,
                amount = loan.Amount,
                status = loan.Status
            });
        }





        // =======================================================
        // 👨‍💼 2. الجزء الخاص بالموظف (EMPLOYEE ENDPOINTS)
        // =======================================================


        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingLoans()
        {
            var requests_pending = await _contextApi.Loans.Where(l => l.Status == "Pending").ToListAsync();
            return Ok(requests_pending);
        }


        [HttpGet("Approved-requests")]
        public async Task<IActionResult> GetApprovedLoans()
        {
            var request_approved = await _contextApi.Loans.Where(loan => loan.Status == "Approved").ToListAsync();
            return Ok(request_approved);
        }







        [HttpPut("{loanId}/approve")]
        public async Task<IActionResult> ApproveLoan(int loanId, [FromBody] LoanReviewDto dto)
        {
            var loan = await _contextApi.Loans.Include(loan => loan.Account).FirstOrDefaultAsync(loan => loan.Id == loanId);
            if (dto.DurationInMonths <= 0 || dto.InterestRate == 0) return BadRequest("يرجى إدخال عدد شهور ونسبة فائدة صحيحة.");
            if (loan == null) return NotFound("طلب القرض غير موجود.");
            if (loan.Status != "Pending") return BadRequest("تم اتخاذ قرار بشأن هذا القرض مسبقاً.");


            decimal totalPayable = loan.Amount + (loan.Amount * dto.InterestRate.Value);

            decimal monthlyInstallment = totalPayable / dto.DurationInMonths.Value;

            loan.DurationInMonths = dto.DurationInMonths.Value;
            loan.InterestRate = dto.InterestRate.Value;
            loan.Status = "Approved";
            loan.TotalAmountPayable = totalPayable;
            loan.MonthlyInstallment = monthlyInstallment;
            loan.Account.Balance += loan.Amount;

            var tranaction = new Transaction
            {
                Amount = loan.Amount,
                TransactionType = "Loan Disbursement",
                AccountId = loan.Account.Id,
                CreatedAt = DateTime.Now

            };

            _contextApi.Transactions.Add(tranaction);
            await _contextApi.SaveChangesAsync();
            return Ok(new
            {
                message = "تمت الموافقة على القرض وإيداع المبلغ في حساب العميل بنجاح.",
                loanId = loan.Id,
                totalPayable = loan.TotalAmountPayable,
                monthlyInstallment = loan.MonthlyInstallment,
                newAccountBalance = loan.Account.Balance
            });
        }




    }
}

