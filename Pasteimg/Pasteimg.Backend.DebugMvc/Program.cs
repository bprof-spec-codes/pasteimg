using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pasteimg.Backend.Configurations;
using Pasteimg.Backend.Data;
using Pasteimg.Backend.DebugMvc.ImageTransformers;
using Pasteimg.Backend.ImageTransformers;
using Pasteimg.Backend.Logic;
using Pasteimg.Backend.Models;
using Pasteimg.Backend.Repository;
using System;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IPasteImgConfigurer, PasteImgConfigurer>()
                .AddSingleton((provider) => provider.GetRequiredService<IPasteImgConfigurer>().ReadConfiguration())
                .AddSingleton((provider) => provider.GetRequiredService<PasteImgConfiguration>().Storage)
                .AddTransient<IImageTransformerFactory, ImageTransformerFactory>()
                .AddSingleton<ImageTransformerTester>()
                .AddTransient<HttpClient>()
                .AddSession();

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
app.MapControllers();
app.Run();