using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB_Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MongoDBService>();

// Ajustando a configuração de MyDatabaseSettings
var myDatabaseSettings = builder.Configuration.GetSection(nameof(MyDatabaseSettings)).Get<MyDatabaseSettings>();

if (myDatabaseSettings == null)
{
    throw new InvalidOperationException("MyDatabaseSettings could not be configured.");
}

builder.Services.AddSingleton(myDatabaseSettings);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

