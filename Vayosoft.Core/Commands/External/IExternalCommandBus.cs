using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Commands.External
{
    public interface IExternalCommandBus
    {
        Task Post<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand;
        Task Put<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand;
        Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T: class, ICommand;
    }
}
