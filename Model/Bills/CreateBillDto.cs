using Banking_System.Model;

namespace Banking.Model.Bills
{
    public class CreateBillDto
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
    }
}
