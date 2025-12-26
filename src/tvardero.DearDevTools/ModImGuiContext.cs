using RWIMGUI.API;
using tvardero.DearDevTools.Components;

namespace tvardero.DearDevTools;

internal sealed class ModImGuiContext : IMGUIContext, IDisposable
{
    private readonly IDearDevToolsPlugin _plugin;
    private bool _disposed;

    public ModImGuiContext(IDearDevToolsPlugin plugin)
    {
        _plugin = plugin;
    }

    public List<ImGuiDrawableBase> RenderList { get; } = [];

    public bool IsActive => ImGUIAPI.CurrentContext == this;

    public void Activate()
    {
        if (!IsActive) ImGUIAPI.SwitchContext(this);
    }

    /// <inheritdoc />
    public override bool BlockWMEvent()
    {
        if (_disposed) return false;
        if (!_plugin.AreDearDevToolsActive) return false;

        bool isMainUiVisible = _plugin.IsMainUiVisible;
        return RenderList
            .Where(drawable => drawable.IsVisible)
            .Where(drawable => isMainUiVisible || !drawable.RequiresMainUiVisible)
            .Any(drawable => drawable.IsBlockingWMEvent);
    }

    public void Deactivate()
    {
        if (IsActive) ImGUIAPI.SwitchContext(null);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Deactivate();

        _disposed = true;
        RenderList.Clear();
    }

    /// <inheritdoc />
    public override void Render(ref IntPtr IDXGISwapChain, ref uint SyncInterval, ref uint Flags)
    {
        if (_disposed) return;
        if (!_plugin.AreDearDevToolsActive) return;

        bool isMainUiVisible = _plugin.IsMainUiVisible;
        IEnumerable<ImGuiDrawableBase> toDraw = RenderList
            .Where(drawable => drawable.IsVisible)
            .Where(drawable => isMainUiVisible || !drawable.RequiresMainUiVisible);

        foreach (ImGuiDrawableBase drawable in toDraw) { drawable.Draw(); }
    }
}