using Banking_System.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banking.Model.Cards
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardType { get; set; }
        [Required]
        public string Cvv { get; set; }

        [Required]
        public string Pin { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsBlocked { get; set; } = false;

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
