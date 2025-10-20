using Application;
using Carter;
using Infrastructure;
using Infrastructure.Data;
using Presentation;
using Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationDependencyInjection();
builder.Services.AddPresentationDependencyInjection();
builder.Services.AddWebDependencyInjection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("LocalPolicy");
}
else
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseCors("ProdPolicy");
}

if (app.Configuration.GetValue<bool>("Database:ApplyMigration"))
{
    await app.Services.MigrateAsync();
}

if (app.Configuration.GetValue<bool>("Database:SeedData"))
{
    await app.Services.SeedAsync();
}

app.MapFallbackToFile("index.html");

app.UseForwardedHeaders();

app.UseRouting();

app.UseRateLimiter();

app.MapCarter(); // Will add with reflection all endpoints which extend the class

app.Run();

namespace Web
{
    public partial class Program { }
}
