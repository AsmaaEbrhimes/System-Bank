using Banking_System.Data;
using Banking_System.Model;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ContextApi _context;


        public CustomersController(ContextApi context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.GetCustomers
                                 .Include(c => c.Accounts)
                                 .ToListAsync();
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
                    .Include(c => c.Accounts)
                    .Where(c => c.Accounts != null && c.Accounts.Any())
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
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);

        }







    }
}
