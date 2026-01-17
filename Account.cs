public class Account
{
    public decimal CreditLimit { get; set; } = 1000.00m; // Default $1000 limit
    public decimal CurrentBalance { get; set; } = 0.00m;
    
    // Calculated property: What's left to spend?
    public decimal AvailableCredit => CreditLimit - CurrentBalance;
}