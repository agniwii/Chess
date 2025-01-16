using Chess_Backend.Hubs;
using Chess_Backend.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowAllOrigins",policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChessService>();
var app = builder.Build();

app.UseCors("AllowAllOrigins");

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseRouting();

app.MapControllers();
app.MapHub<ChessHub>("/chesshub");

app.Run();