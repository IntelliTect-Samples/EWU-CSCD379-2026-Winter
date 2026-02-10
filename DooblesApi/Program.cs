using DooblesApi.Data;
using DooblesApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<DooblesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IDoobleService, DoobleService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Add CORS for your front-end
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
           .AllowAnyMethod()
.AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// Simple root endpoint
app.MapGet("/", () => "DooblesApi");

app.MapControllers();

// Apply migrations on startup
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DooblesDbContext>();
            dbContext.Database.Migrate();
        }
}

app.Run();

public partial class Program { } // For integration testing purposes
