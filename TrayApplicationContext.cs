using System;
using System.Drawing;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly HotkeyWindow _hotkeyWindow;
    private AppSettings _settings;
    private SettingsForm? _settingsForm;

    public TrayApplicationContext()
    {
        _settings = SettingsManager.Load();

        _hotkeyWindow = new HotkeyWindow
        {
            LayoutMode = _settings.LayoutDirection
        };
        _hotkeyWindow.UpdateHotkey(_settings.Hotkey);
        _hotkeyWindow.Show(); // ensures handle is created for hotkey registration
        _hotkeyWindow.Hide();

        ApplyAutostart(_settings.RunAtStartup);

        _trayIcon = new NotifyIcon
        {
            Icon = IconFactory.CreateKeyboardSwapIcon(),
            Visible = true,
            Text = "PuntoSwitcherUkr"
        };

        // Контекстне меню для ПКМ
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Налаштування", null, (_, _) => ShowSettings());
        contextMenu.Items.Add("Вихід", null, (_, _) => ExitApplication());
        _trayIcon.ContextMenuStrip = contextMenu;

        _trayIcon.MouseClick += (_, args) =>
        {
            if (args.Button == MouseButtons.Left)
            {
                ShowSettings();
            }
        };
    }

    private void ShowSettings()
    {
        if (_settingsForm == null || _settingsForm.IsDisposed)
        {
            _settingsForm = new SettingsForm(_settings);
            _settingsForm.SettingsSaved += OnSettingsSaved;
            _settingsForm.ExitRequested += ExitApplication;
        }

        _settingsForm.Show();
        _settingsForm.BringToFront();
        _settingsForm.Focus();
    }

    private void OnSettingsSaved(AppSettings updated)
    {
        _settings = updated;
        _hotkeyWindow.LayoutMode = _settings.LayoutDirection;
        _hotkeyWindow.UpdateHotkey(_settings.Hotkey);
        ApplyAutostart(_settings.RunAtStartup);
        SettingsManager.Save(_settings);
    }

    private static void ApplyAutostart(bool enable)
    {
        var current = AutostartManager.IsEnabled();
        if (current != enable)
        {
            AutostartManager.SetState(enable);
        }
    }

    private void ExitApplication()
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _hotkeyWindow.Close();
        _settingsForm?.Dispose();
        ExitThread();
    }
}
