using Banking.Model.Notifications;
using Banking_System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {

        private readonly ContextApi _context;

        public NotificationsController(ContextApi context)
        {
            _context = context;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerNotifications(int customerId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.CustomerId == customerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Title,
                    n.Message
                })
                .ToListAsync();

            return Ok(notifications);
        }

    }
}

