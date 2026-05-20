using Microsoft.EntityFrameworkCore;
using inter.Models;
using inter.Data;

var builder = WebApplication.CreateBuilder(args);

string connStr =
    "Server=RAFAEL;Database=dbjogal;Trusted_Connection=True;TrustServerCertificate=True;";




// BANCO
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connStr));

// MVC
builder.Services.AddControllersWithViews();

// SESSION
builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();

// HABILITA SESSION
app.UseSession();

// ROTAS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
