using System.Net;
using System.Text.Json;

namespace SmartPanelGuiasApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continúa con la cadena de middlewares
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log del error
                _logger.LogError(ex, "❌ Ocurrió un error inesperado.");

                // Respuesta limpia al cliente
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorObj = new
            {
                error = "Ocurrió un error inesperado en el servidor.",
                detalle = ex.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorObj));
        }
    }
}
