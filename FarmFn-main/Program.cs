using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using mvcbasic.Data;
using Microsoft.AspNetCore.Http;
using Farm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<MvcBasicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FarmContext")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Thêm Authentication và Authorization
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "Farm.Auth";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "1"));
});

var app = builder.Build();

// Khởi tạo Admin mặc định
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MvcBasicDbContext>();
    if (!context.Users.Any(u => u.Role == 1))
    {
        var passwordHasher = new PasswordHasher<User>();
        var admin = new User
        {
            Username = "Admin",
            Name = "Administrator",
            Email = "admin@farm.com",

            Role = 1 // Admin
        };
        admin.Password = passwordHasher.HashPassword(admin, "123"); // Băm mật khẩu với instance của User // mK: 111
        context.Users.Add(admin);
        context.SaveChanges();
    } 
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();  
app.UseAuthentication();
app.UseAuthorization();

// ROUTER
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// UI Routes
app.MapControllerRoute(
    name: "introduce",
    pattern: "{controller=Home}/{action=Introduce}/{id?}");

app.MapControllerRoute(
    name: "news",
    pattern: "{controller=Home}/{action=News}/{id?}");

app.MapControllerRoute(
    name: "privacy",
    pattern: "{controller=Home}/{action=Privacy}/{id?}");

app.MapControllerRoute(
    name: "contact",
    pattern: "{controller=Home}/{action=Contact}/{id?}");

// Admin Routes
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin_login",
    pattern: "Admin/{controller=Auth}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "admin_user_list",
    pattern: "Admin/{controller=User}/{action=List}/{id?}");

app.MapControllerRoute(
    name: "admin_user_edit",
    pattern: "Admin/{controller=User}/{action=Edit}/{id?}");

// Auth Routes
app.MapControllerRoute(
    name: "login",
    pattern: "/login",
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "register",
    pattern: "/register",
    defaults: new { controller = "Auth", action = "Register" });

app.MapControllerRoute(
    name: "shop",
    pattern: "shop",
    defaults: new { controller = "Shop", action = "Index" });

app.MapControllerRoute(
    name: "shop_detail",
    pattern: "shop/{id}",
    defaults: new { controller = "Shop", action = "Detail" });
app.MapControllerRoute(
    name: "cart",
    pattern: "cart",
    defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "checkout",
    pattern: "checkout",
    defaults: new { controller = "Checkout", action = "Index" });

app.MapControllerRoute(
    name: "checkout_success",
    pattern: "success",
    defaults: new { controller = "Checkout", action = "Success" });

app.Run();