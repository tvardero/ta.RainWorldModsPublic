namespace tvardero.DearDevTools.Components;

public abstract class ImGuiDrawableBase
{
    public virtual bool IsVisible { get; set; } = true;

    public virtual bool RequiresMainUiVisible { get; protected set; } = true;

    public virtual bool IsBlockingWMEvent { get; protected set; } = true;

    protected internal abstract void Draw();

    void Show(bool show = true)
    {
        IsVisible = show;
    }

    void Hide()
    {
        IsVisible = false;
    }
}