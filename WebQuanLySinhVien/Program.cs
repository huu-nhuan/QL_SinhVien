using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebQuanLySinhVien.email;
using WebQuanLySinhVien.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddDbContext<QuanLySinhVienContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QLSVConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Access/Login"; 
        options.AccessDeniedPath = "/Home/Error"; 
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "1"));
        options.AddPolicy("GiangVien", policy => policy.RequireClaim("Role", "2"));
        options.AddPolicy("SinhVien", policy => policy.RequireClaim("Role", "3"));
        options.AddPolicy("GiangVienOrAdmin", policy => policy.RequireClaim("Role", "1", "2"));
        options.AddPolicy("SinhVienOrAdmin", policy => policy.RequireClaim("Role", "1", "3"));
    });

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

app.Run();
