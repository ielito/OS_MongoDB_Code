using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB_Code.Models;
using MongoDB_Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSession();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<MongoDBServiceProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Adicione este serviço se ainda não estiver adicionado

// Configure MyDatabaseSettings usando o padrão IOptions
builder.Services.Configure<MyDatabaseSettings>(
    builder.Configuration.GetSection(nameof(MyDatabaseSettings)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
        pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();