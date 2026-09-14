using GymFit.Application.Interfaces;
using GymFit.Application.Mappings;
using GymFit.Domain.Entities;
using GymFit.Infrastructure.Data;
using GymFit.Infrastructure.UnitOfWork;
using GymFit.Services;
using GymFit.Web.Middleware;
using GymFit.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sql => sql.MigrationsAssembly("GymFit.Infrastructure")
    ));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// AutoMapper Configuration
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// AutoMapper Configuration
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile));

// Dependency Injection - Unit of Work & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Logging Service - Custom Error Logging
builder.Services.AddSingleton<ILoggingService, LoggingService>();

// Dependency Injection - Services
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<MembershipService>();

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Add Response Caching
builder.Services.AddResponseCaching();
var app = builder.Build();


// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services);

        // Log successful startup
        var loggingService = services.GetRequiredService<ILoggingService>();
        await loggingService.LogInfoAsync("Application started successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");

        // Also log to custom logging service
        var loggingService = services.GetRequiredService<ILoggingService>();
        await loggingService.LogErrorAsync(ex, "Database seeding failed");
    }
}
// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseResponseCompression();

// Custom Exception Handling Middleware
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCaching();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Route Configuration
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "trainer",
    pattern: "Trainer/{action=Dashboard}/{id?}",
    defaults: new { controller = "Trainers" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
var startupLogger = app.Services.GetRequiredService<ILoggingService>();

app.Run();