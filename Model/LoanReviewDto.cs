namespace Banking.Model
{
    public class LoanReviewDto
    {
        public string Status { get; set; }
        public int? DurationInMonths { get; set; }     // الشهور
        public decimal? InterestRate { get; set; }     // نسبة الفائدة (مثلاً 0.10)
    }
}
