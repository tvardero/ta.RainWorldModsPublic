using tvardero.DearDevTools.Components;
using tvardero.DearDevTools.Services;

namespace tvardero.DearDevTools.Menus;

public class PaletteEditorMenu : SimpleImGuiWindowBase
{
    private readonly PaletteService _paletteService;

    public PaletteEditorMenu(PaletteService paletteService)
    {
        _paletteService = paletteService;
    }

    /// <inheritdoc />
    public override string Name => "Palette selector"; // TODO: Palette editor, see #1

    /// <inheritdoc />
    public override bool IsBlockingWMEvent => false;

    /// <inheritdoc />
    protected override void OnDrawWindowContent() { }
}