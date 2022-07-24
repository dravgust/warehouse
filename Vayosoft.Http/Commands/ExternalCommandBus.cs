using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Commands.External;
using Vayosoft.Http.Policies;
using Polly;

namespace Vayosoft.Http.Commands
{
    public class ExternalCommandBus: IExternalCommandBus
    {
        private readonly HttpClient client;
        private readonly ILogger<ExternalCommandBus> logger;

        public ExternalCommandBus(HttpClient client, ILogger<ExternalCommandBus> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public async Task Post<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            await Send(HttpMethod.Post, url, path, command, cancellationToken);
        }

        public async Task Put<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            await Send(HttpMethod.Put, url, path, command, cancellationToken);
        }

        public async Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            await Send(HttpMethod.Delete, url, path, command, cancellationToken);
        }

        private async Task Send<T>(HttpMethod method, string url, string path, T command, CancellationToken cancellationToken = default)
            where T : class, ICommand
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{url.TrimEnd('/')}/{path.TrimStart('/')}"),
                Content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json")
            };

            var policyContext = new Context()
                .WithLogger(logger);

            request.SetPolicyExecutionContext(policyContext);
            using var httpResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            //httpResponse.EnsureSuccessStatusCode();
        }
    }
}
