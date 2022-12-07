var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigurationServices(builder.Services);

var app = builder.Build();

startup.Configure(app, builder.Environment);

app.Run();