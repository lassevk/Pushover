using System.ComponentModel;

using LVK.Core.App.Console;
using LVK.Core.App.Console.CommandLineInterface;
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

    [CommandLineOption("-v")]
    [CommandLineOption("--verbose")]
    [Description("Show verbose information")]
    public bool Verbose { get; set; }

    [CommandLineOption("--title")]
    [CommandLineOption("-t")]
    [Description("Set title of notification")]
    public string? Title { get; set; }

    [CommandLineOption("--url")]
    [CommandLineOption("-u")]
    [Description("Attach URL to notification")]
    public string? Url { get; set; }

    [CommandLineOption("--url-title")]
    [CommandLineOption("-i")]
    [Description("Specify title of URL")]
    public string? UrlTitle { get; set; }

    [CommandLineOption("--attachment")]
    [CommandLineOption("-a")]
    [Description("Attach file")]
    public string? AttachmentFilePath { get; set; }

    [CommandLineOption("--html")]
    [CommandLineOption("-t")]
    [Description("Notification contains HTML tags")]
    public bool HtmlFormat { get; set; }

    [CommandLineOption("--priority")]
    [CommandLineOption("-p")]
    [Description("Specify priority (lowest, low, normal, high)")]
    public string? Priority { get; set; }

    public async Task<int> RunAsync(CancellationToken stoppingToken)
    {
        var message = string.Join(" ", Messages);

        if (Verbose)
            Console.WriteLine("Sending: " + message);

        var notification = new PushoverNotification(message);
        if (Title != null)
        {
            if (Verbose)
                Console.WriteLine("  with title: " + Title);

            notification = notification.WithTitle(Title);
        }

        if (Url != null)
        {
            string title = UrlTitle ?? Url;
            if (Verbose)
            {
                Console.WriteLine("  with url: " + Url);
                Console.WriteLine("  with url title: " + title);
            }

            notification = notification.WithUrl(Url, title);
        }

        if (AttachmentFilePath != null)
        {
            if (Verbose)
                Console.WriteLine("  with attachment: " + AttachmentFilePath);

            notification = notification.WithAttachmentFromFile(AttachmentFilePath);
        }

        if (HtmlFormat)
        {
            if (Verbose)
                Console.WriteLine("  with html format");

            notification = notification.WithHtmlContent();
        }

        if (Priority != null)
        {
            switch (Priority.ToLowerInvariant())
            {
                case "lowest":
                case "0":
                    notification = notification.WithLowestPriority();
                    if (Verbose)
                        Console.WriteLine("  with lowest priority");

                    break;
                case "low":
                case "1":
                    notification = notification.WithLowPriority();
                    if (Verbose)
                        Console.WriteLine("  with low priority");

                    break;
                case "normal":
                case "2":
                    notification = notification.WithNormalPriority();
                    if (Verbose)
                        Console.WriteLine("  with normal priority");

                    break;
                case "high":
                case "3":
                    notification = notification.WithHighPriority();
                    if (Verbose)
                        Console.WriteLine("  with high priority");

                    break;

                default:
                    throw new InvalidOperationException("Unknown priority value: '" + Priority + "'");
            }
        }

        await _eventBus.PublishAsync(notification, stoppingToken);

        return 0;
    }
}