using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Data;
using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.ImageTransformers._Debug;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));*/

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("test")
           .UseLazyLoadingProxies(true)
           .EnableDetailedErrors(true);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddUserManager<UserManager<IdentityUser>>();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IPasteImgConfigurer, PasteImgConfigurer>();
builder.Services.AddSingleton((provider) => provider.GetRequiredService<IPasteImgConfigurer>().ReadConfiguration());
builder.Services.AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Storage);

builder.Services.AddTransient<IRepository<Image>, Repository<Image>>();
builder.Services.AddTransient<IRepository<Upload>, Repository<Upload>>();
builder.Services.AddSingleton<IFileStorage, FileStorage>();
builder.Services.AddTransient<IImageTransformerFactory, ImageTransformerFactory>();
builder.Services.AddTransient<IPasteImgLogic, PasteImgLogic>();
builder.Services.AddTransient<IPasteImgPublicLogic, PasteImgPublicLogic>();
builder.Services.AddSingleton<ImageTransformerTester>();
builder.Services.AddSession();

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();
app.Run();