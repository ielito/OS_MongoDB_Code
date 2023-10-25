using MongoDB_Code.Models;
using MongoDB_Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurações e serviços
builder.Services.Configure<MyDatabaseSettings>(
    builder.Configuration.GetSection(nameof(MyDatabaseSettings)));

// Adicionar serviços
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSingleton<MongoDBServiceProvider>();
builder.Services.AddLogging();

// Configurar logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Configurações de ambiente
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

// Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();