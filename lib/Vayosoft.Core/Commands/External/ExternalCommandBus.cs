using RestSharp;

namespace Vayosoft.Core.Commands.External
{
    public sealed class ExternalCommandBus: IExternalCommandBus
    {
        public Task Post<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            var client = new RestClient(url);

            var request = new RestRequest(path, Method.Post);
            request.AddJsonBody(command);

            return client.PostAsync<dynamic>(request, cancellationToken);
        }

        public Task Put<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            var client = new RestClient(url);

            var request = new RestRequest(path, Method.Put);
            request.AddJsonBody(command);

            return client.PutAsync<dynamic>(request, cancellationToken);
        }

        public Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand
        {
            var client = new RestClient(url);

            var request = new RestRequest(path, Method.Delete);
            request.AddJsonBody(command);

            return client.DeleteAsync<dynamic>(request, cancellationToken);
        }
    }
}
