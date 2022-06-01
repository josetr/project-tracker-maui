namespace ProjectTracker;

using Microsoft.AspNetCore.Components;

internal class Navigation : INavigation
{
    private readonly NavigationManager navigationManager;

    public Navigation(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public void NavigateTo(string url)
    {
        navigationManager.NavigateTo(url);
    }
}
