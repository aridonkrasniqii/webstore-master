using Microsoft.EntityFrameworkCore;
using WebStore.DAL;
using WebStore.Controllers;
using WebStore.Config;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



var connectionString = builder.Configuration.GetConnectionString("WebStoreContext");
builder.Services.AddDbContext<WebStoreContext>(options =>
    options.UseSqlServer(connectionString));

builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

WebStoreInitializer.CreateDbIfNotExist(app);

app.MapReviewEndpoints();

app.Run();
 