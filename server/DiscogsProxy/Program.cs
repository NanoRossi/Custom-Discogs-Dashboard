using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // localhost 3000 for local development
        // discogs.client.local for kubernetes
        policy.WithOrigins("http://localhost:3000", "http://discogs.client.local")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// TODO: Move this into a dependencyinjection file if this grows more
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IWantListService, WantListService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IDiscogsApiHelper, DiscogsApiHelper>();
builder.Services.AddScoped<IDatabaseChecker, DatabaseChecker>();
builder.Services.AddScoped<IInfoService, InfoService>();
builder.Services.AddScoped<IFactGenerator, FactGenerator>();

// Register EF Core with SQLite
// TODO: move db file into configuration
builder.Services.AddDbContext<DiscogsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.MapControllers();

// For initial startup, we will ensure the database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DiscogsContext>();
    dbContext.Database.EnsureCreated();
}


app.Run();