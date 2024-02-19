using LVK.Core.App.Console;
using LVK.Core.Bootstrapping;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Pushover;

public class ApplicationBootstrapper : IApplicationBootstrapper<HostApplicationBuilder,IHost>
{
    public void Bootstrap(IHostBootstrapper<HostApplicationBuilder, IHost> bootstrapper, HostApplicationBuilder builder)
    {
        bootstrapper.Bootstrap(new LVK.Events.ModuleBootstrapper());
        bootstrapper.Bootstrap(new LVK.Notifications.Pushover.ModuleBootstrapper());

        builder.Services.AddMainEntrypoint<MainEntrypoint>();
        builder.Configuration.AddUserSecrets<Program>();
    }
}