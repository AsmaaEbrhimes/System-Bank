namespace Banking.Model
{
    public class LoanApplyDto
    {
        public decimal Amount { get; set; }
        public int ? AccountId { get; set; }
    }
}
