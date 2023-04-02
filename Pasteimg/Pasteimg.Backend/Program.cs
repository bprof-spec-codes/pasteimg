using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Data;
using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Logic.Exceptions;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Repository;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PasteImgDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .UseLazyLoadingProxies();
});
builder.Services.AddTransient<IPasteImgConfigurer, PasteImgConfigurer>()
                .AddSingleton((provider) => provider.GetRequiredService<IPasteImgConfigurer>().ReadConfiguration())
                .AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Storage)
                .AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Session);
builder.Services.AddSingleton<HttpErrorMapper>();
builder.Services.AddTransient<IRepository<Image>, Repository<Image>>()
                .AddTransient<IRepository<Upload>, Repository<Upload>>()
                .AddTransient<IRepository<Admin>, Repository<Admin>>()
                .AddSingleton<IFileStorage, FileStorage>()
                .AddTransient<IImageTransformerFactory, ImageTransformerFactory>()
                .AddTransient<IPasteImgLogic, PasteImgLogic>()
                .AddSingleton<IDistributedCache, MemoryDistributedCache>()
                .AddTransient<ISessionStore, DistributedSessionStore>()
                .AddTransient<ISessionHandler, SessionHandler>()
                .AddTransient<IPublicLogic, PublicLogic>()
                .AddTransient<IAdminLogic, AdminLogic>();

var app = builder.Build();
app.UseExceptionHandler("/error");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();