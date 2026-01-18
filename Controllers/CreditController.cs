using Microsoft.AspNetCore.Mvc;

namespace revolving_credi_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly ICreditService _creditService;

        public CreditController(ICreditService creditService) 
        { 
            _creditService = creditService; 
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus() => Ok(await _creditService.GetStatusAsync());

        [HttpPost("draw")]
        public async Task<IActionResult> Draw(decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive.");
            try { return Ok(await _creditService.DrawAsync(amount)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("repay")]
        public async Task<IActionResult> Repay(decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be positive.");
            try { 
                return Ok(await _creditService.RepayAsync(amount)); 
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory() => Ok(await _creditService.GetHistoryAsync());
    }
}