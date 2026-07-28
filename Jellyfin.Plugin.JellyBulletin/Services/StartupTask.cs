using System.Reflection;
using System.Runtime.Loader;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JellyBulletin.Services;

/// <summary>
/// Registers the Jellyfin Web index transformation at server startup.
/// </summary>
public sealed class StartupTask : IScheduledTask
{
    private static readonly Guid TransformationId =
        Guid.Parse("33e44c90-c85a-4fe0-a65f-07a57cd8456c");

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

        var pluginType = transformationAssembly?
            .GetType("Jellyfin.Plugin.FileTransformation.FileTransformationPlugin");
        var writeServiceType = transformationAssembly?
            .GetType("Jellyfin.Plugin.FileTransformation.Library.IWebFileTransformationWriteService");
        var transformDelegateType = transformationAssembly?
            .GetType("Jellyfin.Plugin.FileTransformation.Library.TransformFile");
        var serviceProvider = pluginType?
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?
            .GetValue(null)?
            .GetType()
            .GetProperty("ServiceProvider", BindingFlags.Public | BindingFlags.Instance)?
            .GetValue(pluginType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null))
            as IServiceProvider;

        if (writeServiceType is null || transformDelegateType is null || serviceProvider is null)
        {
            _logger.LogWarning(
                "File Transformation was not found. Bulletin cannot be injected into Jellyfin Web.");
            return Task.CompletedTask;
        }

        var writeService = serviceProvider.GetService(writeServiceType);
        var updateMethod = writeServiceType.GetMethod("UpdateTransformation");
        var transformMethod = typeof(WebInjection).GetMethod(
            nameof(WebInjection.TransformIndex),
            BindingFlags.Public | BindingFlags.Static);

        if (writeService is null || updateMethod is null || transformMethod is null)
        {
            _logger.LogWarning(
                "File Transformation does not expose the required transformation API.");
            return Task.CompletedTask;
        }

        var callback = Delegate.CreateDelegate(transformDelegateType, transformMethod);
        updateMethod.Invoke(writeService, [TransformationId, "index.html", callback]);
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
