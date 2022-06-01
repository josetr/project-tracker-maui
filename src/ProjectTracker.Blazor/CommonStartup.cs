namespace ProjectTracker;

using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;

using Components = ProjectTracker.Blazor.Components;

public class CommonStartup
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddMudServices();
        services.AddScoped<TaskEvents>();
        services.AddScoped<StoreState>();
        services.AddScoped<Store>();
        services.AddScoped<ProjectService>();
        services.AddScoped<TaskService>();
        services.AddTransient<IDialogService, DialogService>();
        services.AddTransient<ISnackbar, Snackbar>();
        services.AddTransient<INavigation, Navigation>();
        services.AddTransient<IndexPageViewModel>();
        services.AddTransient<HistoryPageViewModel>();
        services.AddTransient<AddProjectDialogViewModel>();
        services.AddTransient<EditProjectDialogViewModel>();
        services.AddTransient<AddTaskDialogViewModel>();
        services.AddTransient<EditTaskDialogViewModel>();
        services.AddTransient<LayoutViewModel>();
        services.AddScoped<IHotkeyManager, NoHotkeyManager>();
        services.AddTransient<INotificationManager, EmptyNotificationManager>();
        DialogService.Register<AddProjectDialogViewModel, Components.AddProjectDialog>();
        DialogService.Register<EditProjectDialogViewModel, Components.EditProjectDialog>();
        DialogService.Register<AddTaskDialogViewModel, Components.AddTaskDialog>();
        DialogService.Register<EditTaskDialogViewModel, Components.EditTaskDialog>();
    }

    public static MudBlazor.MudTheme CreateDarkPalette()
    {
        return new()
        {
            Palette = new MudBlazor.Palette()
            {
                Black = "#27272f",
                Background = "#000000",
                BackgroundGrey = "#27272f",
                Surface = "#1b1b1b",
                DrawerBackground = "#2B2B2B",
                DrawerText = "rgba(255,255,255, 0.50)",
                DrawerIcon = "rgba(255,255,255, 0.50)",
                AppbarBackground = "#2B2B2B",
                AppbarText = "rgba(255,255,255, 1.0)",
                Primary = "#333333",
                TextPrimary = "rgba(255,255,255, 0.90)",
                TextSecondary = "rgba(255,255,255, 0.5)",
                ActionDefault = "#adadb1",
                ActionDisabled = "rgba(255,255,255, 0.26)",
                ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                Dark = "#c1c1c1"
            }
        };
    }
}
