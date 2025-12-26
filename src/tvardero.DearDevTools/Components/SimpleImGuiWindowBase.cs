using System.Numerics;

namespace tvardero.DearDevTools.Components;

public abstract class SimpleImGuiWindowBase : ImGuiDrawableBase
{
    private bool _isVisible = true;

    /// <inheritdoc />
    public override bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }

    public abstract string Name { get; }

    /// <inheritdoc />
    protected internal sealed override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(600, 400), ImGuiCond.FirstUseEver);
        ImGui.Begin(Name, ref _isVisible);

        OnDrawWindowContent();

        ImGui.End();
    }

    protected abstract void OnDrawWindowContent();
}