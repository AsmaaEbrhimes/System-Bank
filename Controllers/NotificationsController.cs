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

        // =======================================================
        // 1. عرض كل إشعارات العميل (All Notifications)
        // =======================================================

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerNotifications(int customerId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.CustomerId == customerId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }


        // =======================================================
        // 2. عرض عدد الإشعارات غير المقروءة فقط (Unread Count)
        // =======================================================
        [HttpGet("customer/{customerId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int customerId)
        {
            int unreadCount = await _context.Notifications
                .CountAsync(n => n.CustomerId == customerId && !n.IsRead);

            return Ok(new { customerId, unreadCount });
        }

        // =======================================================
        // 3. تحويل الإشعار إلى "تمت القراءة" (Mark as Read)
        // =======================================================
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return NotFound("الإشعار غير موجود.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث حالة الإشعار إلى مقروء.", notificationId = notification.Id });
        }

        // =======================================================
        // 4. تحويل كل إشعارات العميل لـ "تمت القراءة" (Mark All as Read)
        // =======================================================
        [HttpPut("customer/{customerId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(int customerId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.CustomerId == customerId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return Ok(new { message = "لا يوجد إشعارات غير مقروءة." });

            foreach (var item in unreadNotifications)
            {
                item.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعليم جميع الإشعارات كمقروءة." });
        }
    }
}

