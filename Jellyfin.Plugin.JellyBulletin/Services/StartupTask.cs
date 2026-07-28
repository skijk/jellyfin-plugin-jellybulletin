using System.Reflection;
using System.Runtime.Loader;
using Jellyfin.Plugin.JellyBulletin.Models;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Registers the Jellyfin Web index transformation at server startup.
/// </summary>
public sealed class StartupTask : IScheduledTask
{
    private readonly ILogger<StartupTask> _logger;

    public StartupTask(ILogger<StartupTask> logger)
    {
        _logger = logger;
    }

    public string Name => "Bulletin Startup";

    public string Key => "Jellyfin.Plugin.JellyBulletin.Startup";

    public string Description => "Registers Bulletin with Jellyfin Web.";

    public string Category => "Startup Services";

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        var transformationAssembly = AssemblyLoadContext.All
            .SelectMany(context => context.Assemblies)
            .FirstOrDefault(assembly =>
                assembly.FullName?.Contains(".FileTransformation", StringComparison.Ordinal) == true);

        var pluginInterface = transformationAssembly?
            .GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");
        var registerMethod = pluginInterface?.GetMethod("RegisterTransformation");

        if (registerMethod is null)
        {
            _logger.LogWarning(
                "File Transformation was not found. Bulletin cannot be injected into Jellyfin Web.");
            return Task.CompletedTask;
        }

        var payload = new JObject
        {
            ["id"] = "33e44c90-c85a-4fe0-a65f-07a57cd8456c",
            ["fileNamePattern"] = "index.html",
            ["callbackAssembly"] = GetType().Assembly.FullName,
            ["callbackClass"] = typeof(WebInjection).FullName,
            ["callbackMethod"] = nameof(WebInjection.PatchIndex)
        };

        registerMethod.Invoke(null, [payload]);
        _logger.LogInformation("Bulletin registered its Jellyfin Web transformation.");
        progress.Report(100);
        return Task.CompletedTask;
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return
        [
            new TaskTriggerInfo
            {
                Type = TaskTriggerInfoType.StartupTrigger
            }
        ];
    }
}
