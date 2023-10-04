using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB_Code.Services;
using OSMongo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Configure Startup

public void ConfigureServices(IServiceCollection services)
{
    services.Configure<MyDatabaseSettings>(
        Configuration.GetSection(nameof(MyDatabaseSettings)));

    services.AddSingleton<MyDatabaseSettings>(sp =>
        sp.GetRequiredService<IOptions<MyDatabaseSettings>>().Value);

    services.AddSingleton<MongoDBService>();
    ...
}

//Fim Startup

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

