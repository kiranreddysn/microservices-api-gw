namespace OrdersAPI.Middlewares
{
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenLoggingMiddleware> _logger;

        public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("JWT Token: {Token}", token);
            }
            else
            {
                _logger.LogWarning("No JWT Token found in request.");
            }

            await _next(context);
        }
    }

}
