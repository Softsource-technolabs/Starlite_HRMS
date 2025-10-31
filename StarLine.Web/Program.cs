using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using QuestPDF.Infrastructure;
using StarLine.Core.Common;
using StarLine.Infrastructure.Mapping;
using StarLine.Infrastructure.Models;
using StarLine.Web;
using StarLine.Web.Data;
using StarLine.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<StarLiteContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<AppSettings>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(_ => _.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(_ =>
    {
        _.LoginPath = "/Account/Login";
        _.LogoutPath = "/Account/Logout";
        _.AccessDeniedPath = "/Account/AccessDenied";

        _.Cookie.HttpOnly = true;                // No JS access
        _.Cookie.SecurePolicy = CookieSecurePolicy.None; // Force HTTPS
        _.Cookie.SameSite = SameSiteMode.Lax; // Prevent CSRF via cross-site cookies
        _.SlidingExpiration = true;              // Refresh expiration if active
        _.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAuthorization(_ =>
{
    _.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin", "Super-Admin", "HR-Manager"));

    // Employee panel access
    _.AddPolicy("EmployeePolicy", policy =>
        policy.RequireRole("Department-Head", "Employee"));
});

builder.Services.AddDistributedMemoryCache(); // Required for Session

builder.Services.AddSession();

RegisterServices.RegisterService(builder.Services);
builder.Services.AddAutoMapper(_ =>
{
    _.AddProfile<DataProfile>();
});
builder.Services.AddControllersWithViews()
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        ProgressBar = true,
        PositionClass = ToastPositions.TopRight,
        PreventDuplicates = true
    });

QuestPDF.Settings.License = LicenseType.Community;
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAsync(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNToastNotify();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseMiddleware<UserSessionMiddleware>(); // Restore this line
app.UseAuthorization();
app.UseMiddleware<ErrorHandler>();

app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

