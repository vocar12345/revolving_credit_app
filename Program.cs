using Microsoft.EntityFrameworkCore;
using revolving_credi_app;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Database Support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=credit.db"));

// 2. Add Controller Support
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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