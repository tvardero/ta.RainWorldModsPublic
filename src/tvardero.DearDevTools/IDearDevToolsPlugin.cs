using JetBrains.Annotations;

namespace tvardero.DearDevTools;

[PublicAPI]
public interface IDearDevToolsPlugin
{
    /// <summary> Main UI visible. Includes main menu bar, room info panel, room settings panel, and others by default. </summary>
    /// <remarks> Settings this to true will set <see cref="AreDearDevToolsActive" /> to true as well automatically. </remarks>
    bool IsMainUiVisible { get; set; }

    /// <summary>
    /// Quick tools enabled. Includes many utils like 'reset rain timer', 'teleport player', 'kill all creatures' and others by default.
    /// </summary>
    /// <remarks> Setting this to false will set <see cref="IsMainUiVisible" /> to false automatically. </remarks>
    bool AreDearDevToolsActive { get; set; }

    /// <summary>
    /// Service provider used to resolve services and instances of menus.
    /// </summary>
    /// <remarks>
    /// Do not hold on value of this property during mod initialization.
    /// Service provider might be rebuilt multiple times by other dependent mods by call to <see cref="DearDevToolsPlugin.RebuildServiceProvider"/>.
    /// <br/>
    /// Use <see cref="DearDevToolsPlugin.ConfigureServiceProvider"/> as a callback that is executed each time
    /// <see cref="DearDevToolsPlugin.RebuildServiceProvider"/> is called.
    /// </remarks>
    IServiceProvider ServiceProvider { get; }
}