using ConfigurationReader.Core;
using ConfigurationReader.Core.Interfaces;
using ConfigurationReader.Data;
using ConfigurationReader.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs
builder.WebHost.UseUrls("http://0.0.0.0:7000", "https://0.0.0.0:7001");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers with views
builder.Services.AddControllersWithViews();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Memory Cache
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddSingleton<IConfigurationChangeNotifier, NoOpConfigurationNotifier>();

// Register ConfigurationReader as singleton
builder.Services.AddSingleton(sp => new ConfigurationReaderBuilder()
    .SetServiceProvider(sp)
    .SetCache(sp.GetRequiredService<IMemoryCache>())
    .SetLogger(sp.GetRequiredService<ILogger<ConfigurationReader.Core.ConfigurationReader>>())
    .SetApplicationName(builder.Configuration["ApplicationName"] ?? "ConfigurationReader.Web")
    .SetCacheExpiration(TimeSpan.FromMinutes(5))
    .Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Commented out for development
app.UseStaticFiles();

app.UseRouting();
app.UseCors(); // Add CORS middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Configuration}/{action=Index}/{id?}");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
        throw;
    }
}

app.Run();
