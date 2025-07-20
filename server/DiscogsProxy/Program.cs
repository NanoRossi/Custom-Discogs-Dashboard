using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// Register EF Core with SQLite
// TODO: move db file into configuration
builder.Services.AddDbContext<DiscogsContext>(options =>
    options.UseSqlite("Data Source=discogs.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();