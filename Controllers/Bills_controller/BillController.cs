using Banking.Hubs;
using Banking.Model.Bills;
using Banking.Model.Notifications;
using Banking_System.Data;
using Banking_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Banking.Controllers.Bills_controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly ContextApi _context;
        private readonly IHubContext<NotificationHub> _hubContext;


        public BillController(ContextApi context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }



        // =========================================================================
        // 1️- عرض الفواتير المستحقة (غير المدفوعة) لعميل معين
        // =========================================================================
        [HttpGet("customer/{customerId}/unpaid")]
        public async Task<IActionResult> GetUnpaidBills(int customerId)
        {
            var unpaidBills = await _context.Bills
                  .Where(b => b.Account.CustomerId == customerId && b.IsPaid == false)
                  .ToListAsync();
            return Ok(unpaidBills);
        }




        // =========================================================================
        //2- عرض أرشيف الفواتير المدفوعة سابقاً
        // =========================================================================
        [HttpGet("customer/{customerId}/history")]
        public async Task<IActionResult> GetPaidBillsHistory(int customerId)
        {
            var unpaidBills = await _context.Bills
                  .Where(b => b.Account.CustomerId == customerId && b.IsPaid == true)
                  .ToListAsync();
            return Ok(unpaidBills);
        }


        // =========================================================================
        //  إضافة فاتورة جديدة في السيستم (للموظف/السيستم)
        // =========================================================================

        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillDto dto)
        {
            var accountId = await _context.Accounts
                .Where(a => a.CustomerId == dto.CustomerId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();


            if (accountId == 0)
            {
                return NotFound("العميل ده ملوش أي حساب بنكي متسجل في السيستم!");
            }


            var bill = new Bill
            {
                Title = dto.Title,
                Category = dto.Category,
                Amount = dto.Amount,
                DateTime = dto.DateTime,
                IsPaid = false,
                AccountId = accountId
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "تم إضافة الفاتورة للعميل بنجاح!", BillId = bill.Id });
        }



        // =========================================================================
        //3- لتعديل حالة الفاتورة ورصيد الحساب
        // =========================================================================

        [HttpPut("{billId}/pay")]
        public async Task<IActionResult> PayBill(int billId, [FromBody] PayBillDto dto)
        {
            var bill = await _context.Bills.FindAsync(billId);
            var account = await _context.Accounts.FindAsync(bill.AccountId);
            if (bill == null || account == null)
            {
                return NotFound("الفاتورة أو الحساب غير موجود!");
            }

            if (bill.IsPaid)
            {
                return BadRequest("الفاتورة دي مدفوعة بالفعل!");
            }


            if (account.Balance < bill.Amount)
            {
                return BadRequest("الرصيد في الحساب غير كافي لدفع الفاتورة!");
            }

            account.Balance -= bill.Amount;

            bill.IsPaid = true;
            bill.PaidAt = DateTime.Now;
            bill.AccountId = dto.AccountId;
            var transaction = new Banking_System.Model.Transaction
            {
                AccountId = account.Id,
                Amount = bill.Amount,
                TransactionType = "Bill Payment",
                BalanceAfter = account.Balance,
                CreatedAt = DateTime.Now
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();
            string message = $"تم سداد فاتورة ({bill.Title}) بنجاح بمبلغ {bill.Amount} ج.م.";
            var notification = new Notification
            {
                CustomerId = account.CustomerId,
                Title = "سداد الفاتورة بنجاح",
                Message = message,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);


            return Ok(new
            {
                Message = $"تم سداد فاتورة ({bill.Title}) بنجاح بمبلغ {bill.Amount} ج.م.",
                RemainingBalance = account.Balance,
                PaidAt = bill.PaidAt
            });
        }




    }
}
