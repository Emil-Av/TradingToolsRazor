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

ConfigureDatabase(builder);
ConfigureIdentity(builder);
AddServices(builder);

var app = builder.Build();

ApplyMigrations(app);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Use exception handler middleware (handles unhandled exceptions)
app.UseExceptionHandler("/Error");

// Status code pages middleware BEFORE authentication/authorization
app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Redirect the root URL based on authentication status
app.MapGet("/", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        return Results.Redirect("/Home");
    }
    return Results.Redirect("/Account/Login");
});

app.MapRazorPages();

app.Run();

static void ApplyMigrations(WebApplication app)
{
    // Apply database migrations on startup (Production)
    if (!app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        var dbContextFactory = services.GetRequiredService<IDbContextFactory>();
        var dbProvider = app.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

        try
        {
            using var context = dbContextFactory.CreateDbContext();

            logger.LogInformation("Applying {Provider} database migration(s)...", dbProvider);

            var pendingMigrations = context.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migrations", pendingMigrations.Count());
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("  - {Migration}", migration);
                }

                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No migrations to apply.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}

static void AddServices(WebApplicationBuilder builder)
{
    // Configure cookie authentication with persistent login
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Cookie expires after 30 days when RememberMe is checked
        options.SlidingExpiration = true; // Renew cookie on activity
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

    // Register global exception handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Register DbContext factory for migrations
    builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<DeleteTradeService>();
    builder.Services.AddScoped<IStatisticsService, StatisticsService>();
    builder.Services.AddScoped<INewTradeService, NewTradeService>();
    builder.Services.AddScoped<ITradesService, TradesService>();
}

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
    
    string connectionString;
    if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection")
            ?? throw new InvalidOperationException("PostgreSqlConnection string is missing.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.MigrationsAssembly("DataAccess")));
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("SqlServerConnection")
            ?? throw new InvalidOperationException("SqlServerConnection string is missing.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, x => x.MigrationsAssembly("DataAccess")));
    }
}

static void ConfigureIdentity(WebApplicationBuilder builder)
{
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
}