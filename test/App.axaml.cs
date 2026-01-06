using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

using Autodraw.Core.Interfaces;
using Autodraw.Core.Services;

namespace Autodraw;

public partial class App : Application
{
    /* =======================
     *  Dependency Injection
     * ======================= */

    public new static App Current => (App)Application.Current!;
    public IServiceProvider? Services { get; private set; }

    private void ConfigureServices(IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IInputService, InputService>();
        services.AddSingleton<IDrawingService, DrawingService>();

    }

    /* =======================
     *  Theme System (unchanged)
     * ======================= */

    public static string CurrentTheme =
        Config.GetEntry("theme") ?? "avares://Autodraw/Styles/dark.axaml";

    public static bool SavedIsDark =
        Config.GetEntry("isDarkTheme") == null ||
        bool.Parse(Config.GetEntry("isDarkTheme") ?? "true");

    private static int themeIndex = 5;

    private static void ThemeFailed()
    {
        Console.WriteLine("Theme Failed.");
        try
        {
            var resource = (IStyle)AvaloniaXamlLoader.Load(new Uri(CurrentTheme));
            Current.RequestedThemeVariant = SavedIsDark ? ThemeVariant.Dark : ThemeVariant.Light;

            if (Current.Styles.Count > themeIndex)
                Current.Styles.Remove(Current.Styles[themeIndex]);

            Current.Styles.Add(resource);

            Config.SetEntry("theme", CurrentTheme);
            Config.SetEntry("isDarkTheme", SavedIsDark.ToString());
        }
        catch
        {
            Console.WriteLine("Theme Failed 2");

            var resource = (IStyle)AvaloniaXamlLoader.Load(
                new Uri("avares://Autodraw/Styles/dark.axaml"));

            Current.RequestedThemeVariant = ThemeVariant.Dark;

            if (Current.Styles.Count > themeIndex)
                Current.Styles.Remove(Current.Styles[themeIndex]);

            Current.Styles.Add(resource);

            CurrentTheme = "avares://Autodraw/Styles/dark.axaml";
            SavedIsDark = true;

            Config.SetEntry("theme", CurrentTheme);
            Config.SetEntry("isDarkTheme", true.ToString());
        }
    }

    public static string? LoadThemeFromString(string themeText, bool isDark = true, string themeUri = "")
    {
        var output = "";

        try
        {
            themeText = Regex.Replace(themeText, @"file:./", AppDomain.CurrentDomain.BaseDirectory);

            if (!string.IsNullOrEmpty(themeUri))
            {
                themeText = Regex.Replace(
                    themeText,
                    @"style:./",
                    Regex.Replace(themeUri, @"\\(?:.(?!\\))+$", "") + "\\");
            }
            else
            {
                output += "- Theme not saved, style:./ paths may fail.\n\n";
            }

            var isCodeDark = Regex.Match(themeText, @"<!--#DarkTheme-->");
            var isCodeLight = Regex.Match(themeText, @"<!--#LightTheme-->");

            if (isCodeDark.Success && isCodeLight.Success)
                throw new Exception("Theme cannot be both dark and light.");

            if (isCodeDark.Success) isDark = true;
            if (isCodeLight.Success) isDark = false;

            // Avalonia parsing bug workaround
            Console.WriteLine(typeof(Binding));

            var resource = AvaloniaRuntimeXamlLoader.Parse<Styles>(themeText);

            Current.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

            if (Current.Styles.Count > themeIndex)
                Current.Styles.Remove(Current.Styles[themeIndex]);

            Current.Styles.Add(resource);

            if (!string.IsNullOrEmpty(themeUri))
            {
                CurrentTheme = themeUri;
                SavedIsDark = isDark;
                Config.SetEntry("theme", themeUri);
                Config.SetEntry("isDarkTheme", isDark.ToString());
            }
        }
        catch (Exception ex)
        {
            ThemeFailed();
            output += "# Theme failed to load\n" + ex;
            Console.WriteLine(output);
            return output;
        }

        return "# Theme loaded successfully!";
    }

    public static string? LoadTheme(string themeUri, bool isDark = true)
    {
        try
        {
            var resource = (IStyle)AvaloniaXamlLoader.Load(new Uri(themeUri));

            Current.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

            if (Current.Styles.Count > themeIndex)
                Current.Styles.Remove(Current.Styles[themeIndex]);

            Current.Styles.Add(resource);

            CurrentTheme = themeUri;
            SavedIsDark = isDark;

            Config.SetEntry("theme", themeUri);
            Config.SetEntry("isDarkTheme", isDark.ToString());
        }
        catch
        {
            try
            {
                return LoadThemeFromString(File.ReadAllText(themeUri), isDark, themeUri);
            }
            catch (Exception ex)
            {
                ThemeFailed();
                return ex.Message;
            }
        }

        return null;
    }

    /* =======================
     *  Avalonia Lifecycle
     * ======================= */

    public override void Initialize()
    {
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Utils.Log(e.Exception.ToString());
            Utils.Log(e.Exception.Message);
        };

        AvaloniaXamlLoader.Load(this);
        LoadTheme(CurrentTheme, SavedIsDark);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {

            };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
