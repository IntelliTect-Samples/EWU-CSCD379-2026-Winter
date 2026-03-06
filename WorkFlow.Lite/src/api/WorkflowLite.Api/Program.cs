using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkflowLite.Api.Data;
using WorkflowLite.Api.Hubs;
using WorkflowLite.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // (optional) keep defaults; add password rules here if you want
        // options.Password.RequiredLength = 6;
        // options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Services
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

// CORS (read from configuration / environment variables)
// Azure App Setting example:
//   Cors__Origins__0 = https://wonderful-ground-02b56aa10.4.azurestaticapps.net
//   Cors__Origins__1 = http://localhost:3000
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("web", p => p
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // SignalR token support (query string: access_token)
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/workorders"))
                    ctx.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

// Pipeline
app.UseHttpsRedirection();

app.UseRouting();      // important for endpoint metadata (CORS + SignalR)
app.UseCors("web");    // must be before auth

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<WorkOrdersHub>("/hubs/workorders");

// Migrate DB first
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Seed after migrations (roles + admin)
try
{
    await SeedRolesAndAdminAsync(app);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Startup seeding failed (roles/admin). App will continue running.");
}

app.Run();

static async Task SeedRolesAndAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Ensure roles
    foreach (var role in new[] { "Admin", "User" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    // Seed admin
    var email = cfg["SeedAdmin:Email"];
    var password = cfg["SeedAdmin:Password"];

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        return;

    var admin = await userMgr.FindByEmailAsync(email);

    if (admin == null)
    {
        admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var create = await userMgr.CreateAsync(admin, password);

        if (!create.Succeeded)
            throw new Exception("Seed admin create failed: " +
                                string.Join(", ", create.Errors.Select(e => e.Description)));
    }

    // Ensure Admin role membership
    if (!await userMgr.IsInRoleAsync(admin, "Admin"))
        await userMgr.AddToRoleAsync(admin, "Admin");

    // Force password to match seed each startup (great for demos/class)
    var resetToken = await userMgr.GeneratePasswordResetTokenAsync(admin);
    var reset = await userMgr.ResetPasswordAsync(admin, resetToken, password);

    if (!reset.Succeeded)
        throw new Exception("Seed admin password reset failed: " +
                            string.Join(", ", reset.Errors.Select(e => e.Description)));
}