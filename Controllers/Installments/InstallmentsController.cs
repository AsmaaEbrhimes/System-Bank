using Banking.Model;
using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Banking.Controllers.Installments
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallmentsController : ControllerBase
    {

        private readonly ContextApi _contextApi;

        public InstallmentsController(ContextApi context)
        {
            _contextApi = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllInstallments()
        {
            var installments = await _contextApi.Installments.ToListAsync();
            return Ok(installments);
        }



        [HttpGet("{loanId}")]
        public async Task<IActionResult> getInstallmentByLoanId(int loanId)
        {
            var instalmentsbyloanId = await _contextApi.Installments
                .Where(installment => installment.LoanId == loanId)
                .AsNoTracking()
                .ToListAsync();
            return Ok(instalmentsbyloanId);
        }





        [HttpPost]
        public async Task<IActionResult> PayTheInstallment(int idInstallment, [FromBody] DtoPaymentInstallments dtoPayInstallment)
        {
            if (dtoPayInstallment == null || dtoPayInstallment.Amount <= 1)
            {
                return BadRequest("Invalid payment amount. Amount must be greater than zero.");
            }

            var installment = await _contextApi.Installments.FirstOrDefaultAsync(i => i.Id == idInstallment);
            if (installment == null)
            {
                return NotFound("This Installment is Not Found.");
            }

            if (installment.Status == true)
            {
                return BadRequest("This installment has already been paid.");
            }

            // --- الحل هنا: إضافة Include لجلب الـ Account المرتبط بالقرض ---
            var loan = await _contextApi.Loans
                .Include(l => l.Account)
                .FirstOrDefaultAsync(l => l.Id == installment.LoanId);

            if (loan == null)
            {
                return NotFound("Associated loan not found.");
            }

            if (loan.Account == null)
            {
                return NotFound("Associated bank account not found for this loan.");
            }

            installment.Status = true;
            installment.Amount = 0;

            // خصم المبلغ من رصيد الحساب
        //    loan.Account.Balance -= dtoPayInstallment.Amount;

        //    var remainingInstallmentsCount = await _contextApi.Installments
        //.Where(i => i.LoanId == loan.Id && i.Status == false)
        //.CountAsync();

        //    if (remainingInstallmentsCount == 0)
        //    {
        //        //_contextApi.Loans.Remove(loan);
        //    }



            await _contextApi.SaveChangesAsync();
            return Ok(new { message = "Installment paid successfully", remainingBalance = loan.Account.Balance });
        }
    }
}
