using JWTAuthenticationManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddControllers();
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
