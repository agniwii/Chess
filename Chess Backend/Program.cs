using Chess_Backend.Hubs;
using Chess_Backend.Services;
using Chess_Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Konfigurasi CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Izinkan origin frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Izinkan credentials
    });
});

// Register services
builder.Services.AddSingleton<IChessService,ChessService>();
builder.Services.AddSingleton<IRPSService,RPSService>();

// Register SignalR with detailed errors enabled
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers();

var app = builder.Build();

// Gunakan middleware CORS
app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapControllers();
app.MapHub<ChessHub>("/chesshub");

app.Run();