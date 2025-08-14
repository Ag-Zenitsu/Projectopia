using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projectopia.Data;
using Projectopia.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    op => op.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder
    .Services
    .AddIdentity<User, IdentityRole>()
    .AddUserManager<UserManager<User>>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

await SeedData.SeedUsersAndRolesAsync(builder.Services.BuildServiceProvider());
app.Run();
