using Banking_System.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Model
{
    public class Installment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
