using Banking_System.Data;
using Banking_System.Model;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ContextApi _context;


        public CustomersController(ContextApi context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.GetCustomers
                .Select(c => new
                {
                    c.Id,
                    c.FullName,
                    c.NationalId,
                    c.PhoneNumber,
                    c.CreatedAt,
                    HasLoans = c.Accounts.Any(a => _context.Loans.Any(l => l.AccountId == a.Id)),
                    c.Accounts
                })
                .ToListAsync();

            return Ok(customers);
        }







        [HttpGet("Accounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var Accounts = await _context.Accounts.ToListAsync();
            return Ok(Accounts);
        }


        [HttpGet("with-accounts")]
        public async Task<IActionResult> GetCustomersWithAccounts()
        {
            var customers = await _context.GetCustomers
                    .AsNoTracking()
        .Include(c => c.Accounts)
        .Where(c => c.Accounts.Any())
        .ToListAsync();

            return Ok(customers);
        }




        [HttpGet("{Id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int Id)
        {
            var customer = await _context.GetCustomers
                                         .Include(c => c.Accounts)
                                         .FirstOrDefaultAsync(c => c.Id == Id);
            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            _context.GetCustomers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(new
            {
               customer,
                message = "Done add customer successfully"
            });
        }


        [HttpDelete("{id}")]

        public async Task<ActionResult>DeleteCustomer(int id)
        {
            var customerId = await _context.GetCustomers
                .Where(customer => customer.Id == id)
                .ExecuteDeleteAsync();
            if(customerId == 0) return NotFound(new { message = "Card not found" });

            return Ok(new { message = "Done Deleted Customer Successfully" });
        }





    }
}
