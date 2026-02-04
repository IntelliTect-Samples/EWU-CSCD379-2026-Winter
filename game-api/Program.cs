using Microsoft.EntityFrameworkCore;
using game_api.Data;
using game_api.Services;
using System.Data.SqlTypes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddOpenApi();

builder.Services.AddCors(options => { 
    options.AddPolicy("AllowNuxt",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddDbContext<GameDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));
builder.Services.AddScoped<IScoreService, ScoreService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowNuxt");
app.MapControllers(); 

app.Run();