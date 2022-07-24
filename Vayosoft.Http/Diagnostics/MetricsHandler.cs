namespace Vayosoft.Http.Diagnostics
{
    public class MetricsHandler : DelegatingHandler
    {
        public MetricsHandler()
        {
          
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> response; 
            response = base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
