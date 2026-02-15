using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Utilities.Trade;
using Statistics.Services;
using Statistics.Interfaces;
using TradingToolsRazor.Services;
using TradingToolsRazor.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models;
using TradingToolsRazor.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages(options =>
{
    // Require authentication for all pages by default
    options.Conventions.AuthorizeFolder("/");
    
    // Allow anonymous access to Account pages
    options.Conventions.AllowAnonymousToFolder("/Account");
    
    // Allow anonymous access to Error page
    options.Conventions.AllowAnonymousToPage("/Error");
})
.AddNewtonsoftJson(options =>
{
   options.SerializerSettings.Converters.Add(new Shared.TimeOnlyNewtonsoftConverter());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication with persistent login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(365); // Cookie expires after 1 year
    options.SlidingExpiration = true; // Renew cookie on activity
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// Register global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DeleteTradeService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<INewTradeService, NewTradeService>();
builder.Services.AddScoped<ITradesService, TradesService>();

// Allow uploading of files up to 100MB
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Use exception handler middleware (handles unhandled exceptions)
app.UseExceptionHandler("/Error");

// Redirect the root URL to Login page
app.MapGet("/", () => Results.Redirect("/Account/Login"));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Status code pages middleware AFTER authentication/authorization
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.MapRazorPages();

app.Run();
