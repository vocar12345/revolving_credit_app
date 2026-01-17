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
        private static decimal _apr = 0.18m; // This is now used below!

        public CreditController(AppDbContext context) { _context = context; }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            return Ok(new { account.Limit, Balance = Math.Round(account.Balance, 2), Available = Math.Round(account.Limit - account.Balance, 2) });
        }

        [HttpPost("draw")]
        public async Task<IActionResult> Draw(decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive."); // Safety Check
            
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            if (amount > (account.Limit - account.Balance)) return BadRequest("Not enough credit!");

            account.Balance += amount;
            _context.Transactions.Add(new Transaction { Type = "Withdrawal", Amount = amount });
            await _context.SaveChangesAsync();
            return Ok($"Success. New balance: ${Math.Round(account.Balance, 2)}");
        }

        [HttpPost("repay")]
        public async Task<IActionResult> Repay(decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive."); // Safety Check

            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            account.Balance -= amount;
            _context.Transactions.Add(new Transaction { Type = "Repayment", Amount = amount });
            await _context.SaveChangesAsync();
            return Ok($"Success. New balance: ${Math.Round(account.Balance, 2)}");
        }

        [HttpPost("simulate-interest")]
        public async Task<IActionResult> SimulateInterest()
        {
            var account = await _context.Accounts.FirstOrDefaultAsync() ?? await CreateDefaultAccount();
            decimal dailyRate = _apr / 365; // Warning fixed here!
            decimal interestAdded = 0;

            for (int i = 0; i < 30; i++)
            {
                decimal dailyInterest = account.Balance * dailyRate;
                account.Balance += dailyInterest;
                interestAdded += dailyInterest;
            }

            _context.Transactions.Add(new Transaction { Type = "Interest Charge", Amount = Math.Round(interestAdded, 2) });
            await _context.SaveChangesAsync();
            return Ok($"After 30 days, added ${Math.Round(interestAdded, 2)} in interest.");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            return Ok(await _context.Transactions.OrderByDescending(t => t.Date).ToListAsync());
        }

        private async Task<CreditAccount> CreateDefaultAccount()
        {
            var acc = new CreditAccount { Balance = 0, Limit = 1000m };
            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();
            return acc;
        }
    }
}