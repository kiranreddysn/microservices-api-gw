using MassTransit;
using OrdersAPI.Models;
using System.Text.Json;

namespace OrdersAPI.Middlewares
{
    public class MessageConsumer : IConsumer<Order>
    {
        private readonly WebSocketHandler _webSocketHandler;

        public MessageConsumer(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        public async Task Consume(ConsumeContext<Order> context)
        {
            var message = context.Message;
            Console.WriteLine($"Received message from RabbitMQ: {message}");

            var stringMessage = JsonSerializer.Serialize(message);
            await _webSocketHandler.BroadcastMessageAsync(stringMessage);
        }
    }
}
