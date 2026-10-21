using Microsoft.EntityFrameworkCore;

namespace Banking
{
    public static class PaginationExtensions
    {

        public static async Task<object> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int pageNumber = 1,
        int pageSize = 5)
        {
            // 1. حساب إجمالي عدد العناصر
            var totalCount = await query.CountAsync();

            // 2. جلب عناصر الصفحة الحالية
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 3. إرجاع النتيجة مباشرة بدون DTO منفصل
            return new
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = items
            };
        }

    }
}
