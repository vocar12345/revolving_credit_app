# Revolving Credit System API
A .NET 8 Web API demonstrating financial logic, database persistence with SQLite, and transaction auditing.

## Tech Stack
- **Backend:** C# / ASP.NET Core
- **Database:** Entity Framework Core with SQLite
- **Documentation:** Swagger UI

## Interview Highlights
- Implements global exception handling.
- Validates financial inputs (preventing negative draws/repayments).
- Maintains a full transaction history for audit purposes.
## How to Run
1. **Restore Dependencies:** Run `dotnet restore` in the terminal.
2. **Update Database:** Run `dotnet ef database update` to ensure the SQLite schema is ready.
3. **Start the App:** Run `dotnet run`.
4. **Access the Interface:** Open your browser to `http://localhost:5267/swagger` (check your terminal for the exact port).