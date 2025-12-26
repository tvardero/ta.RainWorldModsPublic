using Microsoft.Extensions.DependencyInjection;
using tvardero.DearDevTools.Components;

namespace tvardero.DearDevTools.Services;

public class MenuManager
{
    private readonly ModImGuiContext _modImGuiContext;
    private readonly IServiceProvider _serviceProvider;

    public MenuManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _modImGuiContext = serviceProvider.GetRequiredService<ModImGuiContext>();
    }

    public TDrawable CreateMenu<TDrawable>(bool? isVisible = null)
    where TDrawable : ImGuiDrawableBase
    {
        var menu = ActivatorUtilities.GetServiceOrCreateInstance<TDrawable>(_serviceProvider);
        if (isVisible.HasValue) menu.IsVisible = isVisible.Value;

        if (!_modImGuiContext.RenderList.Contains(menu)) _modImGuiContext.RenderList.Add(menu);

        return menu;
    }

    public IEnumerable<TDrawable> GetExistingMenus<TDrawable>()
    where TDrawable : ImGuiDrawableBase
    {
        return _modImGuiContext.RenderList.OfType<TDrawable>();
    }

    public TDrawable? GetFirstExistingMenu<TDrawable>()
    where TDrawable : ImGuiDrawableBase
    {
        return _modImGuiContext.RenderList.OfType<TDrawable>().FirstOrDefault();
    }
}