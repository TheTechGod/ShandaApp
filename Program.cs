using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShandaApp.Data;
using ShandaApp.Models;
using ShandaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Optional: auto-timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// register AI Services
builder.Services.AddScoped<AIResponseService>();

var app = builder.Build();

// 🔁 SEED information
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User", "Kid" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 🧠 Create default admin
    var adminEmail = "gmetzger1911@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = "gmetz",
            Email = adminEmail,
            FirstName = "Groff",
            LastName = "Metzger",
            AvatarUrl = "/img/avatars/admin/admin-avatar1.png",
            PreferredVoice = "Google US English",
            IsPersonalMode = false
        };

        var result = await userManager.CreateAsync(newAdmin, "ImissShanda23!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}


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

app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//    string[] roles = { "Admin", "User", "Kid" };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }

//    // 🧠 Create default admin
//    var adminEmail = "gmetzger1911@gmail.com";
//    var adminUser = await userManager.FindByEmailAsync(adminEmail);

//    if (adminUser == null)
//    {
//        var newAdmin = new ApplicationUser
//        {
//            UserName = "gmetz",
//            Email = adminEmail,
//            FirstName = "Groff",
//            LastName = "Metzger",
//            AvatarUrl = "/img/avatars/admin/admin-avatar1.png",
//            PreferredVoice = "Google US English",
//            IsPersonalMode = false
//        };

//        var result = await userManager.CreateAsync(newAdmin, "ImissShanda23!");

//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(newAdmin, "Admin");
//        }
//    }
//}

