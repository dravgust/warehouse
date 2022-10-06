using Vayosoft.Core.Commands;
using Vayosoft.Core.Commands.External;

namespace Vayosoft.Testing;

public class DummyExternalCommandBus : IExternalCommandBus
{
    public IList<ICommand> SentCommands { get; } = new List<ICommand>();

    public Task Post<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : class, ICommand
    {
        SentCommands.Add(command);
        return Task.CompletedTask;
    }

    public Task Put<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : class, ICommand
    {
        SentCommands.Add(command);
        return Task.CompletedTask;
    }

    public Task Delete<T>(string url, string path, T command, CancellationToken cancellationToken = default) where T : class, ICommand
    {
        SentCommands.Add(command);
        return Task.CompletedTask;
    }
}
