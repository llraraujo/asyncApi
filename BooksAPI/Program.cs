using BooksAPI.DbContexts;
using BooksAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<BooksContext>(options => 
        options.UseSqlite(builder.Configuration["ConnectionStrings:BooksDBConnectionString"])
    );

builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
