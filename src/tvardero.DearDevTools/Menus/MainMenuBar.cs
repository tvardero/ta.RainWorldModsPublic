using tvardero.DearDevTools.Components;
using tvardero.DearDevTools.Services;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace tvardero.DearDevTools.Menus;

public class MainMenuBar : ImGuiDrawableBase
{
    private readonly MenuManager _menuManager;

    public MainMenuBar(MenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    /// <inheritdoc />
    public override bool IsVisible => true;

    /// <inheritdoc />
    public override bool IsBlockingWMEvent => false;

    /// <inheritdoc />
    public override bool RequiresMainUiVisible => true;

    /// <inheritdoc />
    protected internal override void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Menu"))
            {
                MenuMenu();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                MenuEdit();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View"))
            {
                MenuView();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Navigate"))
            {
                MenuNavigate();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools"))
            {
                MenuTools();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Help"))
            {
                MenuHelp();
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void MenuEdit()
    {
        ImGui.MenuItem("Undo", "Ctrl+Z");
        ImGui.MenuItem("Redo", "Ctrl+Y");

        ImGui.Separator();

        // TODO: history
        ImGui.MenuItem("Clear history");
    }

    private void MenuHelp()
    {
        ImGui.MenuItem("How to use Dear Dev Tools?", "F1");
        ImGui.MenuItem("Whats new?");
        ImGui.MenuItem("Steam Workshop page");

        if (ImGui.MenuItem("GitHub page")) Application.OpenURL("https://github.com/tvardero/tvardero.DearDevTools");

        if (ImGui.MenuItem("Report issue / suggest an idea")) Application.OpenURL("https://github.com/tvardero/tvardero.DearDevTools/issues");

        if (ImGui.MenuItem("Support development")) Application.OpenURL("https://ko-fi.com/tvardero");

        ImGui.Separator();

        if (ImGui.MenuItem("Escape the end", "Esc + End")) Utils.ForceCrash(ForcedCrashCategory.Abort);
    }

    private void MenuMenu()
    {
        ImGui.MenuItem("Mod editor");
        ImGui.MenuItem("Region editor");
        ImGui.MenuItem("Dialog editor");
        ImGui.MenuItem("Map editor");
        ImGui.MenuItem("Palette editor");

        ImGui.Separator();

        ImGui.MenuItem("Settings");
    }

    private void MenuNavigate()
    {
        ImGui.MenuItem("Warp to region/room");
        ImGui.MenuItem("Warp back");

        ImGui.Separator();

        ImGui.MenuItem("Sleep screen");
        ImGui.MenuItem("Death screen");
        ImGui.MenuItem("Main menu");
    }

    private void MenuTools()
    {
        ImGui.MenuItem("Cycle control");
        ImGui.MenuItem("Creatures control");
        ImGui.MenuItem("Kill all creatures except slugcat", "Ctrl+K");

        ImGui.Separator();

        ImGui.MenuItem("Room settings");
        ImGui.MenuItem("Palette editor");
        ImGui.MenuItem("Room effects");
        ImGui.MenuItem("Room objects");
        ImGui.MenuItem("Room sounds");
        ImGui.MenuItem("Room triggers");
    }

    private static void MenuView()
    {
        ImGui.MenuItem("RW Debug");
        ImGui.MenuItem("ImGui Debug");
    }
}