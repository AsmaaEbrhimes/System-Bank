using System.ComponentModel.DataAnnotations;

namespace Banking.Model.Bills
{
    public class PayBillDto
    {
        [Required]
        public int AccountId { get; set; }
    }
}
