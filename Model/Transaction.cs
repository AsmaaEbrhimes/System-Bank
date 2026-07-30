using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking_System.Model
{
    public class Transaction
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TransactionType { get; set; } // Deposit, Withdraw, Transfer
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int AccountId { get; set; }
        public Account ? Account { get; set; }
    }
}
