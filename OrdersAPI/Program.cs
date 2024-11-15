using JWTAuthenticationManager;
using MassTransit;
using OrdersAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host("rabbitmq://rabbitmq:5672", h =>
        {
            h.Username("guest"); // Default username
            h.Password("guest"); // Default password
        });

        cfg.ReceiveEndpoint("q1", c => {
            c.ConfigureConsumer<MessageConsumer>(context);
        });

        // Add consumers, queues, etc.
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<WebSocketHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenLoggingMiddleware>();


app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        Console.WriteLine($"User is authenticated: {context.User.Identity.Name}");
    }
    else
    {
        Console.WriteLine("User is not authenticated");
    }
    await next();
});

app.Run();
