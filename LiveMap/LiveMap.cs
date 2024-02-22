using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Server;
using CitizenFX.Server.Native;
using LiveMap.Extensions;
using LiveMap.Handlers;
using LiveMap.Tasks;
using LiveMap.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LiveMap;

public class LiveMap : BaseScript
{
    private static LogLevel? _loglevelEnum;

    private static string? _resourceName;

    private readonly ILogger<LiveMap> _logger;
    private readonly ServiceProvider _serviceProvider;
    private readonly List<ITask> _tasks = new();

    public LiveMap()
    {
        var services = new ServiceCollection();
        services.AddSingleton(this);
        services.AddLogging(builder => builder
            .SetMinimumLevel(LiveMapLogLevel)
            .AddProvider(new DebugLoggerProvider())
        );

        services.AddSingletonClasses()
            .AddTasks();

        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger<LiveMap>>();

        Natives.SetConvarServerInfo("sv_livemap:version", Constants.LiveMapVersion.ToString());

        SetUp();
    }

    private static LogLevel LiveMapLogLevel
    {
        get
        {
            // If LogLevel is already set, return it
            if (_loglevelEnum != null) return _loglevelEnum.Value;
            var logLevel = Config.GetConfigKeyValue(Constants.Config.Debug, 0, "off");

            switch (logLevel)
            {
                case "[all]":
                case "trace":
                    _loglevelEnum = LogLevel.Trace;
                    break;
                case "debug":
                    _loglevelEnum = LogLevel.Debug;
                    break;
                case "info":
                    _loglevelEnum = LogLevel.Information;
                    break;
                case "warn":
                    _loglevelEnum = LogLevel.Warning;
                    break;
                case "error":
                    _loglevelEnum = LogLevel.Error;
                    break;
                case "critical":
                    _loglevelEnum = LogLevel.Critical;
                    break;
                default:
                    _loglevelEnum = LogLevel.None;
                    break;
            }

            return _loglevelEnum.Value;
        }
    }

    public static string ResourceName => _resourceName ??= Natives.GetCurrentResourceName();

    private void SetUp()
    {
        AddTasks();

        // Set up the HTTP handler... Apparently this must be done before the fist tick of the resource...
        var httpHandler = _serviceProvider.GetRequiredService<BasicHttpHandler>();

        // EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);

        // Client to server events
        // EventHandlers["livemap:blipsGenerated"] +=
        //     new Action<Player, Dictionary<int, List<Blip>>>(blipHandler.OnBlipsGenerated);
        // EventHandlers["livemap:AddBlip"] += new Action<Player, Blip>(blipHandler.AddBlip);
        // EventHandlers["livemap:UpdateBlip"] += new Action<Player, Blip>(blipHandler.UpdateBlip);

        Natives.SetHttpHandler(httpHandler.OnHttpEndpointRequest);

        // Natives.RegisterCommand("blips", new Action<int, List<object>>(blipHandler.BlipsCommand), true);
        // Natives.RegisterCommand("blips", new Action<Player, List<object>>(blipHandler.BlipsCommand), true);

        foreach (var task in _tasks) RegisterTick(task.Execute);

        AddCommands();
        AddEventHandlers();
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
        //var blipHandler = _serviceProvider.GetRequiredService<BlipHandler>();
        //Natives.RegisterCommand("blips",blipHandler.BlipsCommand, true);
        //Natives.RegisterCommand("blips", blipHandler.BlipsCommandFromPlayer, true);
    }

    private void AddEventHandlers()
    {
        // Get all methods that have the [EventHandler] attribute and add them to the event handlers except the ones that in this class
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .SelectMany(p => p.GetMethods())
            .Where(m => m.GetCustomAttributes<EventHandlerAttribute>().Any() && m.DeclaringType != typeof(LiveMap));

        foreach (var method in methods)
        {
            var eventAttribute = method.GetCustomAttribute<EventHandlerAttribute>();
            if (eventAttribute == null) continue;

            _logger.LogDebug("Adding event handler for {Event} with binding {Binding}", eventAttribute.Event,
                eventAttribute.Binding);

            EventHandlers[eventAttribute.Event]
                .Add(Func.Create(method.DeclaringType, method), eventAttribute.Binding);
        }
    }

    // ReSharper disable once UnusedMember.Local
    [Command("blip")]
    private void BlipCommand(int source, object[] args)
    {
        var blipHandler = _serviceProvider.GetRequiredService<BlipHandler>();
        blipHandler.BlipsCommand(source, args);
    }

    // ReSharper disable once UnusedMember.Local
    [Command("blip", RemapParameters = true)]
    private void BlipCommand([Source] Player source, object[] args)
    {
        var blipHandler = _serviceProvider.GetRequiredService<BlipHandler>();
        blipHandler.BlipsCommandFromPlayer(source, args);
    }

    // ReSharper disable once UnusedMember.Local
    [EventHandler("onResourceStop")]
    private void OnResourceStop(string resourceName)
    {
        if (resourceName != ResourceName) return;

        _logger.LogDebug("LiveMap resource stopped. Cleaning up...");
        _serviceProvider.GetRequiredService<BlipHandler>().SaveBlips();
        _serviceProvider.Dispose();
    }
}