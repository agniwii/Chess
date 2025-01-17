using Chess_Backend.Hubs;
using Chess_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Konfigurasi CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
            // policy.AllowAnyOrigin()
        policy.WithOrigins("http://localhost:5173") // Izinkan origin frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Izinkan credentials
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChessService>();

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