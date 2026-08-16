using System.Configuration;
using System.Data;
using System.Windows;
using H4KeymapEditor.Models;

namespace H4KeymapEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public Settings AppSettings { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        AppSettings = Settings.Load();
        if (AppSettings.UseDarkMode)
            Themes.ThemeManager.SwapTheme();
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        AppSettings.Save();
        base.OnExit(e);
    }
}

