using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.Extensions;
using Hospital_Management_System.Helper;
using Hospital_Management_System.Middlewares;
using Hospital_Management_System.Repository.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAppServices();
// Register the DbContext with SQL Server using the connection string from configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DC"));
});


// Add Swagger services
builder.Services.AddSwaggerServices();
var app = builder.Build();

var scope = app.Services.CreateScope();
var services= scope.ServiceProvider;
var logger = services.GetRequiredService<ILoggerFactory>();
try
{
    // Apply migrations for both DbContexts

    var HospitalDbContext =services.GetRequiredService<ApplicationDbContext>();
    await HospitalDbContext.Database.MigrateAsync();
    
   

    // Seed the admin user
    var userManager = services.GetRequiredService<UserManager<UserApp>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedAdmin.AddAdmin(userManager, roleManager);
}
catch (Exception ex)
{
    var log = logger.CreateLogger<Program>();
    log.LogError(ex, "An error occurred while migrating the database or seeding data.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerServices();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
