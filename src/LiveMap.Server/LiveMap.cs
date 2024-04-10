using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Server.Native;
using LiveMap.Server.Extensions;
using LiveMap.Server.Handlers;
using LiveMap.Server.Tasks;
using LiveMap.Server.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server;

public class LiveMap : BaseScript
{
    private static string? _resourceName;

    private readonly ILogger<LiveMap> _logger;
    private readonly ServiceProvider _serviceProvider;
    private readonly List<ITask> _tasks = [];

    public LiveMap()
    {
        var services = new ServiceCollection();
        services
            .AddSingleton(this)
            .AddSingletonClasses()
            .AddTasks()
            .AddCommands();
        
        services.AddLogging(builder => builder
            .SetMinimumLevel(Config.LiveMapLogLevel)
            .AddProvider(new DebugLoggerProvider())
        );
        
        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger<LiveMap>>();

        Natives.SetConvarServerInfo("sv_livemap:version", Constants.LiveMapVersion.ToString());

        SetUp();
    }

    public static string ResourceName => _resourceName ??= Natives.GetCurrentResourceName();

    private void SetUp()
    {
        AddTasks();
        AddCommands();
        AddEventHandlers();

        // Set up the HTTP handler... Apparently this must be done before the fist tick of the resource...
        var httpHandler = _serviceProvider.GetRequiredService<BasicHttpHandler>();
        Natives.SetHttpHandler(httpHandler.OnHttpEndpointRequest);

        foreach (var task in _tasks)
        {
            _logger.LogDebug("Registering task {TaskName}", task.GetType().Name);
            RegisterTick(task.Execute);
        }
    }

    private void AddTasks()
    {
        // Search for all classes that implement ITask and get their instance from the ServiceProvider and add them to the tasks list
        var taskTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ITask).IsAssignableFrom(p)
                        && !p.IsInterface && !p.IsAbstract);

        foreach (var taskType in taskTypes)
        {
            var task = (ITask?)_serviceProvider.GetService(taskType);
            if (task == null) continue;

            _tasks.Add(task);
        }
    }

    private void AddCommands()
    {
        // Get all methods where their class extends BaseScript and has the CommandAttribute.
        // Have to do it this was because FiveM doesn't allow us to use the DynFunc.CreateCommand method.
        // So, we have to force their Initialization method to be ran which, calls the CreateCommand method: https://github.com/citizenfx/fivem/blob/37639cddcb2e5b0ef05f96d5a482b236b7349a1e/code/client/clrcore-v2/BaseScript.cs#L97
        
        // Get all classes who has atleast one method with the CommandAttribute
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.GetMethods().Any(m =>
                m.GetCustomAttribute<CommandAttribute>() != null))
            .ToArray();

        _logger.LogDebug("Found {CommandCount} commands to register", methods.Length);
        
        foreach (var method in methods)
        {
            if (method.DeclaringType == null || method.DeclaringType == typeof(LiveMap)) continue;

            _logger.LogDebug("Getting instance for {Type}", method.DeclaringType.Name);
            var instance = (BaseScript?)_serviceProvider.GetService(method.DeclaringType);
            RegisterScript(instance);
        }

        // foreach (var method in methods)
        // {
        //     var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
        //     if (commandAttribute == null || method.DeclaringType == null) continue;
        //
        //     var instance = _serviceProvider.GetService(method.DeclaringType);
        //
        //     _logger.LogDebug(
        //         "Adding command handler for {Command}, with Restricted: {Restricted} & Remap: {RemapParams}",
        //         commandAttribute.Command, commandAttribute.Restricted, commandAttribute.RemapParameters);
        //
        //     var del = Reflection.CreateDelegateFrom(method, instance);
        //     
        //     Natives.RegisterCommand(commandAttribute.Command,
        //         Func.Create(del), commandAttribute.Restricted);
        // }

        //var blipHandler = _serviceProvider.GetRequiredService<BlipHandler>();
        //Natives.RegisterCommand("blips",blipHandler.BlipsCommand, true);
        //Natives.RegisterCommand("blips", blipHandler.BlipsCommandFromPlayer, true);
    }

    private void AddEventHandlers()
    {
        // Get all methods that have the [EventHandler] attribute and add them to the event handlers except the ones that in this class
        var methods = Reflection.GetMethodsWithAttribute<EventHandlerAttribute>();

        foreach (var method in methods)
        {
            var eventAttribute = method.GetCustomAttribute<EventHandlerAttribute>();
            if (eventAttribute == null || method.DeclaringType == null) continue;

            var instance = _serviceProvider.GetService(method.DeclaringType);

            var del = Reflection.CreateDelegateFrom(method, instance);

            _logger.LogDebug(
                "Registering EventHandler {EventName} ({Binding}) for: {MethodName}({Params})",
                eventAttribute.Event, eventAttribute.Binding, method.Name,
                string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name)));

            EventHandlers[eventAttribute.Event].Add(Func.Create(del), eventAttribute.Binding);
        }
    }

    // ReSharper disable once UnusedMember.Local
    [EventHandler("onResourceStop")]
    private void OnResourceStop(string resourceName)
    {
        if (resourceName != ResourceName) return;

        _logger.LogDebug("LiveMap resource stopped. Cleaning up...");
        
        // Get all IDsposeable instances and dispose them
        foreach (var disposable in _serviceProvider.GetServices<IDisposable>())
        {
            disposable.Dispose();
        }
        
        //_serviceProvider.GetRequiredService<BlipHandler>().SaveBlips();
        _serviceProvider.Dispose();
    }
}