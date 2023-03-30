using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Data;
using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models.Entity;
using Pasteimg.Backend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("test")
           .UseLazyLoadingProxies(true)
           .EnableDetailedErrors(true);
});

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddUserManager<UserManager<IdentityUser>>();

builder.Services.AddSession();

builder.Services.AddTransient<IPasteImgConfigurer, PasteImgConfigurer>();
builder.Services.AddSingleton((provider) => provider.GetRequiredService<IPasteImgConfigurer>().ReadConfiguration());
builder.Services.AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Storage);
builder.Services.AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Session);

builder.Services.AddTransient<IRepository<Image>, Repository<Image>>();
builder.Services.AddTransient<IRepository<Upload>, Repository<Upload>>();
builder.Services.AddSingleton<IFileStorage, FileStorage>();
builder.Services.AddTransient<IImageTransformerFactory, ImageTransformerFactory>();
builder.Services.AddTransient<IPasteImgLogic, PasteImgLogic>();
builder.Services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
builder.Services.AddSingleton<ISessionHandler, SessionHandler>();
builder.Services.AddTransient<IPasteImgPublicLogic, PasteImgPublicLogic>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
app.MapControllers();

app.Run();
