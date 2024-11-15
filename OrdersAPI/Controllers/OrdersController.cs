using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Middlewares;
using OrdersAPI.Models;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly WebSocketHandler _webSocketHandler;


        public OrdersController(IPublishEndpoint publishEndpoint, WebSocketHandler webSocketHandler)
        {
            _publishEndpoint = publishEndpoint;
            _webSocketHandler = webSocketHandler;
        }
        // GET: api/<OrdersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "order1", "order2" };
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async void Post([FromBody] Order order)
        {
            Debug.WriteLine(Request.HttpContext);
            await _publishEndpoint.Publish<Order>(order);
            Accepted();
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("live-orders")]
        public async Task GetLiveOrders()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _webSocketHandler.HandleWebSocketAsync(HttpContext);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
