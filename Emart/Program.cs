using Emart.Data;
using EMart.Repositories.Implementation;
using EMart.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// Add services to the container
// =====================================================

builder.Services.AddControllersWithViews();


// =====================================================
// Database
// =====================================================

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DBCS")));


// =====================================================
// Repositories
// =====================================================

builder.Services.AddScoped<ICategoriesRepository,
    CategoriesRepository>();

builder.Services.AddScoped<ISubCategoriesRepository,
    SubCategoriesRepository>();

builder.Services.AddScoped<IBrandsRepository,
    BrandsRepository>();

builder.Services.AddScoped<IRolesRepository,
    RolesRepository>();

builder.Services.AddScoped<IUsersRepository,
    UsersRepository>();

builder.Services.AddScoped<IUnitsRepository,
    UnitsRepository>();

builder.Services.AddScoped<IProductsRepository,
    ProductsRepository>();

builder.Services.AddScoped<ICartRepository,
    CartRepository>();

builder.Services.AddScoped<IAddressRepository,
    AddressRepository>();

builder.Services.AddScoped<IAccountRepository,
    AccountRepository>();
builder.Services.AddScoped<IOrderRepository,
    OrderRepository>();


// =====================================================
// Cookie Authentication -- Admin, Customer, Vendor, DilveryBoys
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            "CustomerCookie";

        options.DefaultSignInScheme =
            "CustomerCookie";

        options.DefaultChallengeScheme =
            "CustomerCookie";
    })
    // Customer Cookie
    .AddCookie("CustomerCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(30);

        options.SlidingExpiration = true;

        options.Cookie.Name = "EMart.Customer";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    })

    // Admin Cookie
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(30);

        options.SlidingExpiration = true;

        options.Cookie.Name = "EMart.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });


// =====================================================
// Session
// =====================================================

builder.Services.AddSession(options =>
{
    options.IdleTimeout =
        TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});


var app = builder.Build();


// =====================================================
// HTTP Request Pipeline
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();


// Routing should come before Session/Auth
app.UseRouting();


// Session
app.UseSession();


// Authentication
app.UseAuthentication();


// Authorization
app.UseAuthorization();


// =====================================================
// Static Assets
// =====================================================

app.MapStaticAssets();


// =====================================================
// Areas Route
// =====================================================

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


// =====================================================
// Default Route
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();