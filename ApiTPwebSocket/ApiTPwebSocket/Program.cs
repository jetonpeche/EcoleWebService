using ApiTPwebSocket.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(x => x.AddDefaultPolicy(y => y.WithOrigins().WithHeaders().WithMethods()));

// ajout de signalr + voir les erreurs
builder.Services.AddSignalR(option => 
{
    option.EnableDetailedErrors = true;
});

var app = builder.Build();

app.UseCors();
    
// route API pour acceder au hub
app.MapHub<PixelHub>("/pixel");

app.Run();
