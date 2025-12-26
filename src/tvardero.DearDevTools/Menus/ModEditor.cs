using tvardero.DearDevTools.Components;

namespace tvardero.DearDevTools.Menus;

// TODO
public class ModEditor : SimpleWindowWithLeftPanelBase
{
    private string? _selectedModId;

    private readonly SortedSet<ModInfo> _coreMods = new(Comparer<ModInfo>.Create((x, y) => string.CompareOrdinal(x.Id, y.Id)));
    private readonly SortedSet<ModInfo> _userMods = new(Comparer<ModInfo>.Create((x, y) => string.CompareOrdinal(x.Id, y.Id)));
    private readonly SortedSet<ModInfo> _steamWorkshopMods = new(Comparer<ModInfo>.Create((x, y) => string.CompareOrdinal(x.Id, y.Id)));

    /// <inheritdoc />
    public override string Name => "Mod Editor";

    /// <inheritdoc />
    public ModEditor()
    {
        ReloadList();
    }

    public void ReloadList()
    {
        // list core mods (hardcoded list)

        // list user mods (from mods/ directory, excluding core mods)

        // list steam workshop mods
    }

    /// <inheritdoc />
    protected override void OnDrawLeftPanel()
    {
        ImGui.Text("Core mods");

        // show core mods (watcher, msc, devtools, remix, etc.)
        // - do not allow modifying modinfo for any core mod
        // - show button to open files directory of the mod
        ListMods(_coreMods);

        ImGui.Separator();
        ImGui.Text("User mods");

        // show user mods from local mods/ directory
        // - allow modifying modinfo for local user mods
        // - show button to open steam workshop management
        // - show button to open files directory of the mod
        // - add options to create a file watcher for source directory of the mod
        ListMods(_userMods);

        ImGui.Separator();
        ImGui.Text("Steam Workshop mods");

        // show mods from steam workshop
        // - do not allow modifying modinfo for any steam workshop mod
        // - show button to open workshop page
        // - show button to open files directory of the mod (so if user wants - they can modify mod manually)
        ListMods(_steamWorkshopMods);

        return;

        void ListMods(IEnumerable<ModInfo> mods)
        {
            foreach (var mod in mods) ImGui.Selectable(mod.Name + "##" + mod.Id, mod.Id == _selectedModId);
        }
    }

    /// <inheritdoc />
    protected override void OnDrawMiddleContent()
    {
        ImGui.BeginGroup();

        ImGui.EndGroup();
    }

    public record ModInfo(string Id, string Name, bool IsCore, bool IsUser, bool IsSteam, bool IsEnabled, string ModAbsolutePath)
    {
        public bool CanEditModInfo => IsUser;
    }
}