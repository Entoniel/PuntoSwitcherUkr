using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal enum LayoutMode
{
    EnToUa,
    UaToEn
}

internal sealed class AppSettings
{
    public LayoutMode LayoutDirection { get; set; } = LayoutMode.EnToUa;
    public bool RunAtStartup { get; set; }
    public HotkeyConfig Hotkey { get; set; } = HotkeyConfig.Default();
}

internal sealed class HotkeyConfig
{
    public bool Control { get; set; } = true;
    public bool Alt { get; set; }
    public bool Shift { get; set; } = true;
    public int Key { get; set; } = (int)Keys.F4;

    public static HotkeyConfig Default() => new();
}

internal static class SettingsManager
{
    private const string SettingsFileName = "settings.json";

    private static string ConfigDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PuntoSwitcherUkr");

    private static string SettingsPath => Path.Combine(ConfigDirectory, SettingsFileName);

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    loaded.Hotkey ??= HotkeyConfig.Default();
                    return loaded;
                }
            }
        }
        catch
        {
            // ignored: fall back to defaults below
        }

        return new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(ConfigDirectory);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // swallowing errors to avoid crashing tray app; optionally log here
        }
    }
}
