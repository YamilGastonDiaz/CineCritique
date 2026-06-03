using CineCritique.Data;
using CineCritique.Models;
using CineCritique.Service;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Incluir DbContext
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//add identity
builder.Services.AddIdentityCore<Usuario>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
}
)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MovieDbContext>()
    .AddSignInManager();

//Manejo de la cookie. Lo ponemos en default, pero hay que ponerlo.
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = IdentityConstants.ApplicationScheme;
})
.AddIdentityCookies();


builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    o.SlidingExpiration = true;
    o.LoginPath = "/Usuario/Login";
    o.AccessDeniedPath = "/Usuario/AccessDenied";
});

//Servicios de archivos
builder.Services.AddScoped<ImagenStorage>();
builder.Services.Configure<FormOptions>(o => { o.MultipartBodyLengthLimit = 2 * 1024 * 1024; });

//Servicios de mail
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

//Servicios LLM
builder.Services.AddScoped<LlmService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MovieDbContext>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbSeeder.Seed(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error ocurred seeding the DB.");
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
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
