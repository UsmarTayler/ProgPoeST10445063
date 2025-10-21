using CMCS.Mvc.Data;                 // <-- add
using Microsoft.EntityFrameworkCore;  // <-- add

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// EF Core DbContext (LocalDB)
builder.Services.AddDbContext<CmcsContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CMCS")));



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Claims}/{action=Index}/{id?}");

app.Run();
