using System.Numerics;

namespace tvardero.DearDevTools.Components;

public abstract class SimpleWindowWithLeftPanelBase : SimpleImGuiWindowBase
{
    private int _selected = 0;

    /// <inheritdoc />
    protected sealed override void OnDrawWindowContent()
    {
        ImGui.BeginChild("Left pane", new Vector2(150, 0), ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX);
        OnDrawLeftPanel();
        ImGui.EndChild();

        ImGui.SameLine();
        OnDrawMiddleContent();
    }

    public bool IsLeftPanelCollapsed { get; set; }

    protected abstract void OnDrawLeftPanel();

    protected abstract void OnDrawMiddleContent();
}