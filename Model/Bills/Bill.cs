using Banking_System.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Model.Bills
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }     //تاريخ استحقاق الفاتورة
        public bool IsPaid { get; set; }     //حالة الدفع (false = غير مدفوعة, true = مدفوعة)
        public DateTime? PaidAt { get; set; }     //تاريخ وساعة الدفع
        [Required]
        public int AccountId { get; set; }     //العميل الموجهة له الفاتورة
        public Account Account { get; set; }
    }
}
