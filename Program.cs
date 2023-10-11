using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB_Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(); // Adiciona o serviço de sessão
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddDistributedMemoryCache(); // Adicione este serviço se ainda não estiver adicionado

// Configure MyDatabaseSettings usando o padrão IOptions
builder.Services.Configure<MyDatabaseSettings>(
    builder.Configuration.GetSection(nameof(MyDatabaseSettings)));

// Adicionando MongoDBServiceProvider e MongoDBService ao container de serviços
//builder.Services.AddScoped<MongoDBServiceProvider>();
builder.Services.AddSingleton<MongoDBServiceProvider>();


var app = builder.Build();

// Configure o pipeline de solicitação HTTP.
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
// Adiciona o middleware de sessão
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Backoffice}/{action=Index}/{id?}");

app.Run();