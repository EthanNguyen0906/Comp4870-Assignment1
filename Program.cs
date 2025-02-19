using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Assignment1.Data; // Fixed namespace
using Assignment1.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with relaxed password rules
builder.Services.AddDefaultIdentity<User>(options => 
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Other services
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        
        // Seed Admin
        var adminEmail = "a@a.a";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                Approved = true
            };
            await userManager.CreateAsync(adminUser, "P@$$w0rd");
        }

        // Seed Contributor
        var contributorEmail = "c@c.c";
        if (await userManager.FindByEmailAsync(contributorEmail) == null)
        {
            var contributorUser = new User
            {
                Email = contributorEmail,
                FirstName = "Contributor",
                LastName = "User",
                Role = "Contributor",
                Approved = true
            };
            await userManager.CreateAsync(contributorUser, "P@$$w0rd");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding users");
    }
}

app.Run();

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage) 
        => Task.CompletedTask;
}