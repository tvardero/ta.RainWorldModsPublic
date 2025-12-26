using BepInEx;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using tvardero.DearDevTools.Menus;
using tvardero.DearDevTools.Services;
using UnityEngine;

namespace tvardero.DearDevTools;

/// <summary>
/// Dear Dev Tools mod.
/// </summary>
[BepInPlugin("tvardero.DearDevTools", "Dear Dev Tools", "0.0.7")]
[BepInDependency("rwimgui")]
[PublicAPI]
public sealed class DearDevToolsPlugin : BaseUnityPlugin, IDisposable, IDearDevToolsPlugin
{
    private static DearDevToolsPlugin? _instance;
    private static bool _skipOnModsInit;
    private static readonly List<Action<IServiceCollection>> _configureServiceCollection = [];
    private static readonly List<Action<IServiceProvider>> _configureServiceProvider = [];

    private ServiceProvider _serviceProvider = null!;
    private ModImGuiContext _modImGuiContext = null!;
    private MenuManager _menuManager = null!;

    /// <summary>
    /// Singleton instance of fully initialized Dear Dev Tools mod.
    /// </summary>
    /// <exception cref="InvalidOperationException"> Dear Dev Tools mod is not initialized. </exception>
    public static DearDevToolsPlugin Instance => _instance ?? throw new InvalidOperationException("Dear Dev Tools mod is not initialized");

    /// <summary>
    /// Is Dear Dev Tools mod initialized.
    /// </summary>
    public static bool IsInitialized => _instance != null;

    /// <summary> Main UI visible. Includes main menu bar, room info panel, room settings panel, and others by default. </summary>
    /// <remarks> Settings this to true will set <see cref="AreDearDevToolsActive" /> to true as well automatically. </remarks>
    public bool IsMainUiVisible
    {
        get => field && AreDearDevToolsActive;

        set
        {
            if (value == field) return;

            if (value) AreDearDevToolsActive = true;
            field = value;

            if (value) _modImGuiContext.Activate();
        }
    }

    /// <summary>
    /// Quick tools enabled. Includes many utils like 'reset rain timer', 'teleport player', 'kill all creatures' and others by default.
    /// </summary>
    /// <remarks> Setting this to false will set <see cref="IsMainUiVisible" /> to false automatically. </remarks>
    public bool AreDearDevToolsActive
    {
        get => field || IsMainUiVisible;

        set
        {
            if (value == field) return;

            if (!value)
            {
                _modImGuiContext.Deactivate();
                IsMainUiVisible = false;
            }

            field = value;

            if (value) _modImGuiContext.Activate();
        }
    }

    /// <summary>
    /// Service provider used to resolve services and instances of menus.
    /// </summary>
    /// <remarks>
    /// Do not hold on value of this property during mod initialization.
    /// Service provider might be rebuilt multiple times by other dependent mods by call to <see cref="RebuildServiceProvider"/>.<br/>
    /// Use <see cref="ConfigureServiceProvider"/> as a callback that is executed each time <see cref="RebuildServiceProvider"/> is called.
    /// </remarks>
    public IServiceProvider ServiceProvider => _serviceProvider;

    [UsedImplicitly]
    private void Update()
    {
        if (_instance != this) return;

        // todo: make configurable

        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool hPressed = Input.GetKeyDown(KeyCode.H);
        bool oPressed = Input.GetKeyDown(KeyCode.O);

        if (ctrlPressed && oPressed)
        {
            bool willBeActive = !AreDearDevToolsActive;
            Logger.LogInfo(willBeActive ? "Activating Dear Dev Tools" : "Deactivating Dear Dev Tools");
            AreDearDevToolsActive = willBeActive;
        }

        if (AreDearDevToolsActive && ctrlPressed && hPressed)
        {
            bool willBeVisible = !IsMainUiVisible;
            Logger.LogInfo(willBeVisible ? "Showing main UI" : "Hiding main UI");
            IsMainUiVisible = willBeVisible;
        }
    }

    [UsedImplicitly]
    private void OnEnable()
    {
        Logger.LogInfo("OnEnable called, registering initialization callback");

        if (_skipOnModsInit) Initialize();
        else On.RainWorld.OnModsInit += OnModsInit;
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        Logger.LogInfo("OnDisable called, deinitializing mod instance");

        if (_instance == this) _instance = null;

        On.RainWorld.OnModsInit -= OnModsInit;

        _modImGuiContext.Dispose();
    }

    /// <summary>
    /// Disposes (destroys) current mod instance.
    /// </summary>
    public void Dispose()
    {
        Logger.LogInfo("Dispose called, deinitializing mod instance");

        if (_instance == this) _instance = null;

        On.RainWorld.OnModsInit -= OnModsInit;

        _serviceProvider.Dispose();
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        Initialize();
        _skipOnModsInit = true;
    }

    /// <summary>
    /// Register or override services for service provider.<br/>
    /// After registering all services, call <see cref="RebuildServiceProvider"/> to rebuild service provider.
    /// </summary>
    /// <remarks>
    /// Dear Dev Tools does not use Scoped lifetime, it uses only Singleton and Transient.
    /// If you want to use Scoped lifetime - go ahead, but note that you need to handle scopes yourself.
    /// </remarks>
    /// <param name="configure">Configure action.</param>
    /// <exception cref="ArgumentNullException">Configure action is null.</exception>
    public static void ConfigureServiceCollection(Action<IServiceCollection> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        if (!_configureServiceCollection.Contains(configure)) _configureServiceCollection.Add(configure);
    }

    /// <summary>
    /// Register callback to resolve services from rebuilt service provider and additionally configure them.<br/>
    /// Called everytime service provider is rebuilt by <see cref="RebuildServiceProvider"/> method.
    /// </summary>
    /// <param name="configure">Configure action.</param>
    /// <exception cref="ArgumentNullException">Configure action is null.</exception>
    public static void ConfigureServiceProvider(Action<IServiceProvider> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        if (!_configureServiceProvider.Contains(configure)) _configureServiceProvider.Add(configure);
    }

    /// <summary>
    /// Rebuilds service provider. Resets the Dear Dev Tools mod.
    /// </summary>
    public void RebuildServiceProvider()
    {
        Logger.LogInfo("Rebuilding service provider");

        // NOTE: multiple downstream dependents might call this method multiple times 

        var serviceCollection = new ServiceCollection();

        foreach (Action<IServiceCollection> configure in _configureServiceCollection)
        {
            try { configure(serviceCollection); }
            catch (Exception e) { Logger.LogError($"Error while executing service collection configure action (pre-build)\n{e}"); }
        }

        ConfigureDefaults(serviceCollection);

        ServiceProvider serviceProvider;
        try
        {
            serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }
        catch (Exception e)
        {
            Logger.LogError($"Error while building service provider, did you register all necessary services?\n{e}");
            throw;
        }

        foreach (Action<IServiceProvider> configure in _configureServiceProvider)
        {
            try { configure(serviceProvider); }
            catch (Exception e) { Logger.LogError($"Error while executing service provider configure action (post-build)\n{e}"); }
        }

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _serviceProvider?.Dispose();
        _serviceProvider = serviceProvider;

        _modImGuiContext = _serviceProvider.GetRequiredService<ModImGuiContext>();
        _menuManager = _serviceProvider.GetRequiredService<MenuManager>();

        ResetMod();
    }

    private void ResetMod()
    {
        Logger.LogInfo("Resetting Dear Dev Tools");

        AreDearDevToolsActive = true; // TODO: true is temporary
        IsMainUiVisible = false;

        _menuManager.CreateMenu<DearDevToolsEnabledOverlay>();
        _menuManager.CreateMenu<MainMenuBar>();
    }

    private void ConfigureDefaults(ServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDearDevToolsPlugin>(this);
        serviceCollection.AddSingleton<ModImGuiContext>();
        serviceCollection.AddSingleton<MenuManager>();
        serviceCollection.AddSingleton<DearDevToolsEnabledOverlay>();
        serviceCollection.AddSingleton<MainMenuBar>();
        serviceCollection.AddSingleton<GameStateService>();
    }

    private void Initialize()
    {
        Logger.LogInfo("Initializing mod instance");

        if (_instance == this) return;

        try { RebuildServiceProvider(); }
        catch (Exception e)
        {
            Logger.LogFatal($"Fatal error during Dear Dev Tool initialization\n{e}");
            throw;
        }

        _instance = this;
        Logger.LogInfo("Initialization complete;");
    }
}