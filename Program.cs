using Microsoft.EntityFrameworkCore;
using PaulaPresentesWebMVC.Data;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".PaulaPresentes.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

app.Urls.Add($"http://*:{port}");

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();