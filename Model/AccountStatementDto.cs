namespace Banking.Model
{
    public class AccountStatementDto
    {
      
            public int AccountId { get; set; }
            public string AccountNumber { get; set; }
            public decimal CurrentBalance { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public decimal TotalDeposits { get; set; }
            public decimal TotalWithdrawals { get; set; }
            public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        }

        public class TransactionDto
        {
            public int TransactionId { get; set; }
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal BalanceAfter { get; set; }
            public DateTime Date { get; set; }
        }

        public class CustomerSummaryDto
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int TotalAccountsCount { get; set; }
            public decimal TotalBalanceAcrossAccounts { get; set; }
            public int ActiveCardsCount { get; set; }
            public decimal TotalActiveLoansAmount { get; set; }
            public decimal TotalRemainingLoansAmount { get; set; }
        }
    }

