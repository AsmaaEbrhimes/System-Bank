using Banking.Model.Notifications;
using Banking_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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



        [HttpGet("GetAllNotification")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var all_notifications = await _context.Notifications.ToListAsync();
            return Ok(all_notifications);
        }



        [HttpPut("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead()
        {
            var UnReadNotifications = await _context.Notifications
                .Where(notificate => !notificate.IsRead)
                .ToListAsync();

            foreach (var item in UnReadNotifications)
            {
                item.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Notifications marked as read" });
        }
    }
}

