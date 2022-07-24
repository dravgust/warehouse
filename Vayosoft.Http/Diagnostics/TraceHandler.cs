using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Vayosoft.Http.Diagnostics
{
    public class TraceHandler : DelegatingHandler
    {
        private readonly ILogger<TraceHandler> _logger;

        public TraceHandler(ILogger<TraceHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.ToString();
            string body = null;
            if(request.Content != null)
                body = await request.Content.ReadAsStringAsync(cancellationToken);
            var fullRequest = $"{headers}\r\n{body}";

            var options = new JsonSerializerOptions { WriteIndented = true };
            _logger.LogInformation($"{JsonSerializer.Serialize(fullRequest, options)}");

            var response = await base.SendAsync(request, cancellationToken);

            headers = response.ToString();
            body = await response.Content.ReadAsStringAsync(cancellationToken);
            var fullResponse = $"{headers}\r\n{body}";

            _logger.LogInformation($"{JsonSerializer.Serialize(fullResponse, options)}");

            return response;
        }
    }
}
