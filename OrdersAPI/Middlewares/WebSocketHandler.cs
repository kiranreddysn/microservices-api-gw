using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace OrdersAPI.Middlewares
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

        public async Task HandleWebSocketAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var connectionId = Guid.NewGuid().ToString();

                _connections.TryAdd(connectionId, webSocket);

                try
                {
                    await ReceiveMessagesAsync(webSocket, connectionId);
                }
                finally
                {
                    _connections.TryRemove(connectionId, out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request if it's not a WebSocket request
            }
        }

        private async Task ReceiveMessagesAsync(WebSocket webSocket, string connectionId)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                // Process and broadcast the received message
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {receivedMessage}");

                // Broadcast the message to all clients
                await BroadcastMessageAsync($"{connectionId}: {receivedMessage}");
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);

            foreach (var connection in _connections.Values)
            {
                if (connection.State == WebSocketState.Open)
                {
                    await connection.SendAsync(
                        new ArraySegment<byte>(messageBuffer),
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        cancellationToken: CancellationToken.None);
                }
            }
        }
    }
}
