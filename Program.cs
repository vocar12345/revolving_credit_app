using Microsoft.EntityFrameworkCore;
using revolving_credi_app;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Database Support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=credit.db"));

// for service
builder.Services.AddScoped<ICreditService, CreditService>();
// 2. Add Controller Support
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("A system error occurred. Our engineers are notified.");
    }
});
// 3. Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. THIS IS THE MOST IMPORTANT LINE:
app.MapControllers();

app.Run();