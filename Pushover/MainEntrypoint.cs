using LVK.Core.App.Console;
using LVK.Core.App.Console.Parameters;
using LVK.Events;
using LVK.Notifications.Pushover;

namespace Pushover;

public class MainEntrypoint : IMainEntrypoint
{
    private readonly IEventBus _eventBus;

    public MainEntrypoint(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    [PositionalArguments]
    public List<string> Messages { get; } = new();

    public async Task<int> RunAsync(CancellationToken stoppingToken)
    {
        var message = string.Join(" ", Messages);

        Console.WriteLine("Sending: " + message);

        await _eventBus.PublishAsync(new PushoverNotification(message), stoppingToken);


        return 0;
    }
}