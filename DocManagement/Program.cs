using DocManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Verilənlər bazasına bağlantı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity xidmətləri - default UI olmadan
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    response.ContentType = "text/html; charset=utf-8";

    string title = "";
    string message = "";

    switch (response.StatusCode)
    {
        case StatusCodes.Status403Forbidden:
            title = "İcazə yoxdur";
            message = "Bu əməliyyatı etmək üçün hüququnuz yoxdur.";
            break;

        case StatusCodes.Status404NotFound:
            title = "Səhifə tapılmadı";
            message = "Bu əməliyyatı etmək üçün hüququnuz yoxdur və ya axtardığınız səhifə mövcud deyil.";
            break;

        default:
            return; // digər statuslara qarışmırıq
    }

    await response.WriteAsync($@"
        <html>
            <head><title>{title}</title></head>
            <body style='text-align:center; margin-top:100px; font-family:sans-serif;'>
                <h2 style='color:red;'>{message}</h2>
                <a href='/' style='text-decoration:none; color:#007bff;'>Ana səhifəyə qayıt</a>
            </body>
        </html>
    ");
});



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Documents}/{action=Index}/{id?}");


// --- Yeni əlavə ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string adminRoleName = "Admin";

    // Admin rolu yoxdursa yarat
    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
    }

    // Default admin istifadəçi yaradılır (əgər yoxdursa)
    string adminUserName = "admin";
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createUserResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
        }
    }
}
// --- Yeni əlavə bitdi ---

app.Run();
