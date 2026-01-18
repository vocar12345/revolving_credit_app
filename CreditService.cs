using Microsoft.EntityFrameworkCore;

namespace revolving_credi_app
{
    // The Interface (The "Contract")
    public interface ICreditService
    {
        Task<object> GetStatusAsync();
        Task<string> DrawAsync(decimal amount);
        Task<string> RepayAsync(decimal amount);
        Task<string> SimulateInterestAsync();
        Task<List<Transaction>> GetHistoryAsync();
    }

    // The Service (The "Chef" who does the work)
    public class CreditService : ICreditService
    {
        private readonly AppDbContext _context;
        private static decimal _apr = 0.18m;

        public CreditService(AppDbContext context) { _context = context; }

        public async Task<object> GetStatusAsync()
        {
            var account = await GetOrCreateAccount();
            return new { account.Limit, Balance = Math.Round(account.Balance, 2), Available = Math.Round(account.Limit - account.Balance, 2) };
        }

        public async Task<string> DrawAsync(decimal amount)
        {
            var account = await GetOrCreateAccount();
            if (amount > (account.Limit - account.Balance)) throw new Exception("Not enough credit!");

            account.Balance += amount;
            _context.Transactions.Add(new Transaction { Type = "Withdrawal", Amount = amount });
            await _context.SaveChangesAsync();
            return $"Success. New balance: ${Math.Round(account.Balance, 2)}";
        }

        public async Task<string> RepayAsync(decimal amount)
        {
            var account = await GetOrCreateAccount();

            // Check if the repayment amount exceeds the current debt
            if (amount > Math.Round(account.Balance, 2))
            {
                throw new Exception($"Overpayment not allowed. You only owe ${Math.Round(account.Balance, 2)}.");
            }

        account.Balance -= amount;
        _context.Transactions.Add(new Transaction { Type = "Repayment", Amount = amount });
        await _context.SaveChangesAsync();
    
        return $"Success. New balance: ${Math.Round(account.Balance, 2)}";
        }

        public async Task<string> SimulateInterestAsync()
        {
            var account = await GetOrCreateAccount();
            decimal dailyRate = _apr / 365;
            decimal interestAdded = 0;
            for (int i = 0; i < 30; i++) {
                decimal dailyInterest = account.Balance * dailyRate;
                account.Balance += dailyInterest;
                interestAdded += dailyInterest;
            }
            _context.Transactions.Add(new Transaction { Type = "Interest Charge", Amount = Math.Round(interestAdded, 2) });
            await _context.SaveChangesAsync();
            return $"Added ${Math.Round(interestAdded, 2)} interest.";
        }

        public async Task<List<Transaction>> GetHistoryAsync()
        {
            return await _context.Transactions.OrderByDescending(t => t.Date).ToListAsync();
        }

        private async Task<CreditAccount> GetOrCreateAccount()
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync();
            if (acc == null) {
                acc = new CreditAccount { Balance = 0, Limit = 1000m };
                _context.Accounts.Add(acc);
                await _context.SaveChangesAsync();
            }
            return acc;
        }
    }
}