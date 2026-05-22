using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// PORTA DO RAILWAY
// ===============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.UseUrls($"http://*:{port}");

// ===============================
// SERVICES
// ===============================

builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});


// Database PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(60);
        }));

// ===============================
// BUILD
// ===============================
var app = builder.Build();

// ===============================
// MIDDLEWARES
// ===============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (CryptographicException)
    {
        context.Response.Cookies.Delete(".PaulaPresentes.Session");
        context.Response.Redirect(context.Request.Path);
    }
});

app.UseSession();

app.UseAuthorization();

// ===============================
// ROUTES
// ===============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===============================
// RUN
// ===============================

app.Run();