using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();



app.MapControllers();
await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
