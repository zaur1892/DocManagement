using EvtapAz.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(EvtapAz.SharedResource));
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

var supportedCultures = new[]
{
    new CultureInfo("az"),
    new CultureInfo("ru"),
    new CultureInfo("en"),
};

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("az"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    response.ContentType = "text/html; charset=utf-8";

    (string title, string msg) = response.StatusCode switch
    {
        404 => ("Səhifə tapılmadı", "Axtardığınız səhifə mövcud deyil."),
        403 => ("İcazə yoxdur", "Bu əməliyyat üçün hüququnuz yoxdur."),
        _   => ("Xəta", "Xəta baş verdi.")
    };

    await response.WriteAsync($@"
        <html><head><meta charset='utf-8'><title>{title}</title>
        <style>body{{font-family:sans-serif;text-align:center;margin-top:100px;background:#f8f9fc}}
        h2{{color:#1E3A5F}}a{{color:#F4A92A;text-decoration:none;font-weight:bold}}</style></head>
        <body><h2>{title}</h2><p>{msg}</p><a href='/'>Ana Səhifəyə Qayıt</a></body></html>");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    try { await DatabaseSeeder.SeedAsync(scope.ServiceProvider); }
    catch (Exception ex) { Console.WriteLine($"Seed xətası: {ex.Message}"); }
}

app.Run();
