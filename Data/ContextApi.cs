
using Banking.Model;
using Banking.Model.Bills;
using Banking.Model.Cards;
using Banking_System.Model;
using Microsoft.EntityFrameworkCore;

namespace Banking_System.Data
{
    public class ContextApi:DbContext
    {

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Customer> GetCustomers { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }






        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer("Server=.;Database=Banking;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
