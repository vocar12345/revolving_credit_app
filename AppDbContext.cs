using Microsoft.EntityFrameworkCore;

namespace revolving_credi_app
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<CreditAccount> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; } 
    }

    public class CreditAccount
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public decimal Limit { get; set; } = 1000m;
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; } = ""; 
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}