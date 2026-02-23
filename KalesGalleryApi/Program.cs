using Azure.Storage.Blobs;
using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<GalleryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with built-in API endpoints (provides /register, /login, /refresh, etc.)
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<GalleryDbContext>();

builder.Services.AddAuthorization();

// Configure CORS for the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Azure Blob Storage
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration["AzureBlob:ConnectionString"]));

// Register application services
builder.Services.AddScoped<IArtPieceService, ArtPieceService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<ICommissionTypeService, CommissionTypeService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Apply pending migrations automatically (required for Azure hosting)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
        await db.Database.MigrateAsync();
    }
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map built-in Identity endpoints: /register, /login, /refresh, /confirmEmail, /manage/*
app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.MapGet("/", () => "KalesGalleryApi");

app.Run();
