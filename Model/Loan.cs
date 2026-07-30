using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking_System.Model
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal Amount { get; set; }                // المبلغ المطلوب
        public int? DurationInMonths { get; set; }         // بيحدده الموظف
        public decimal? InterestRate { get; set; }         // بيحدده الموظف (مثلاً 0.10)
        public decimal? TotalAmountPayable { get; set; }   // بيحسبه السيستم
        public decimal? MonthlyInstallment { get; set; }   // بيحسبه السيستم
        public decimal? RemainingAmount { get; set; }      // المتبقي للسداد

        public string Status { get; set; } = "Pending";     // Pending, Approved, Rejected, Completed
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        // ربط القرض بالحساب
        public int? AccountId { get; set; }
        public Account Account { get; set; }
    }
}