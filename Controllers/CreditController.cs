using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using revolving_credi_app;

namespace revolving_credi_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly AppDbContext _context;
        private static decimal _apr = 0.18m;

        public CreditController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var account = await _context.Accounts.FirstOrDefaultAsync();
            if (account == null) 
            {
                account = new CreditAccount { Balance = 0, Limit = 1000m };
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
            }

            return Ok(new { 
                account.Limit, 
                Balance = Math.Round(account.Balance, 2), 
                Available = Math.Round(account.Limit - account.Balance, 2) 
            });
        }

        [HttpPost("draw")]
        public async Task<IActionResult> Draw(decimal amount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();

            if (amount > (account.Limit - account.Balance)) return BadRequest("Not enough credit!");

            account.Balance += amount;
            
            // LOG TO HISTORY
            _context.Transactions.Add(new Transaction { Type = "Withdrawal", Amount = amount });
            
            await _context.SaveChangesAsync();
            return Ok($"Success. New balance: ${Math.Round(account.Balance, 2)}");
        }

        [HttpPost("repay")]
        public async Task<IActionResult> Repay(decimal amount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            account.Balance -= amount;

            // LOG TO HISTORY
            _context.Transactions.Add(new Transaction { Type = "Repayment", Amount = amount });

            await _context.SaveChangesAsync();
            return Ok($"Success. New balance: ${Math.Round(account.Balance, 2)}");
        }

        [HttpPost("simulate-interest")]
        public async Task<IActionResult> SimulateInterest()
        {
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            decimal dailyRate = _apr / 365;
            decimal totalInterest = 0;

            for (int i = 0; i < 30; i++)
            {
                decimal dailyInterest = account.Balance * dailyRate;
                account.Balance += dailyInterest;
                totalInterest += dailyInterest;
            }

            // LOG TO HISTORY
            _context.Transactions.Add(new Transaction { Type = "Interest Charge", Amount = Math.Round(totalInterest, 2) });

            await _context.SaveChangesAsync();
            return Ok($"Added ${Math.Round(totalInterest, 2)} interest.");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.Date)
                .ToListAsync();
            return Ok(transactions);
        }

        // Helper to keep code clean
        private async Task<CreditAccount> CreateDefaultAccount()
        {
            var acc = new CreditAccount { Balance = 0, Limit = 1000m };
            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();
            return acc;
        }
    }
}