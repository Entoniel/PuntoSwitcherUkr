using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal sealed class SettingsForm : Form
{
    private readonly ComboBox _layoutCombo;
    private readonly CheckBox _autostartCheck;
    private readonly Label _hotkeyLabel;
    private readonly Button _hotkeyButton;
    private readonly Button _saveButton;
    private readonly Button _exitButton;
    private bool _exiting;
    private AppSettings _settings;

    public event Action<AppSettings>? SettingsSaved;
    public event Action? ExitRequested;

    public SettingsForm(AppSettings settings)
    {
        _settings = new AppSettings
        {
            LayoutDirection = settings.LayoutDirection,
            RunAtStartup = settings.RunAtStartup,
            Hotkey = new HotkeyConfig
            {
                Control = settings.Hotkey.Control,
                Alt = settings.Hotkey.Alt,
                Shift = settings.Hotkey.Shift,
                Key = settings.Hotkey.Key
            }
        };

        Text = "Налаштування PuntoSwitcherUkr";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(420, 220);

        var layoutLabel = new Label
        {
            Text = "Мова для зміни розкладки:",
            Location = new Point(15, 15),
            AutoSize = true
        };

        _layoutCombo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(18, 40),
            Width = 320
        };
        _layoutCombo.Items.Add(new LayoutChoice("Англійська → Українська", LayoutMode.EnToUa));
        _layoutCombo.Items.Add(new LayoutChoice("Українська → Англійська", LayoutMode.UaToEn));
        _layoutCombo.SelectedIndex = _settings.LayoutDirection == LayoutMode.EnToUa ? 0 : 1;

        _hotkeyLabel = new Label
        {
            Text = $"Гаряча клавіша: {FormatHotkey(_settings.Hotkey)}",
            Location = new Point(15, 75),
            AutoSize = true
        };

        _hotkeyButton = new Button
        {
            Text = "Змінити гарячу клавішу",
            Location = new Point(18, 100),
            Width = 200
        };
        _hotkeyButton.Click += (_, _) => ChangeHotkey();

        _autostartCheck = new CheckBox
        {
            Text = "Автозапуск при вході в систему",
            Location = new Point(18, 140),
            AutoSize = true,
            Checked = _settings.RunAtStartup
        };

        _saveButton = new Button
        {
            Text = "Зберегти",
            Location = new Point(30, 170),
            Width = 150
        };
        _saveButton.Click += (_, _) => SaveSettings(hideAfterSave: true);

        _exitButton = new Button
        {
            Text = "Закрити програму",
            Location = new Point(230, 170),
            Width = 160
        };
        _exitButton.Click += (_, _) => ExitApp();

        Controls.Add(layoutLabel);
        Controls.Add(_layoutCombo);
        Controls.Add(_hotkeyLabel);
        Controls.Add(_hotkeyButton);
        Controls.Add(_autostartCheck);
        Controls.Add(_saveButton);
        Controls.Add(_exitButton);
    }

    private void SaveSettings(bool hideAfterSave)
    {
        var selected = _layoutCombo.SelectedItem as LayoutChoice;
        _settings.LayoutDirection = selected?.Mode ?? LayoutMode.EnToUa;
        _settings.RunAtStartup = _autostartCheck.Checked;
        // hotkey already stored in _settings when changed

        SettingsSaved?.Invoke(new AppSettings
        {
            LayoutDirection = _settings.LayoutDirection,
            RunAtStartup = _settings.RunAtStartup,
            Hotkey = new HotkeyConfig
            {
                Control = _settings.Hotkey.Control,
                Shift = _settings.Hotkey.Shift,
                Alt = _settings.Hotkey.Alt,
                Key = _settings.Hotkey.Key
            }
        });

        if (hideAfterSave)
        {
            Hide();
        }
    }

    private void ExitApp()
    {
        SaveSettings(hideAfterSave: false);
        _exiting = true;
        ExitRequested?.Invoke();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_exiting)
        {
            e.Cancel = true;
            Hide();
            return;
        }

        base.OnFormClosing(e);
    }

    private sealed record LayoutChoice(string Caption, LayoutMode Mode)
    {
        public override string ToString() => Caption;
    }

    private static string FormatHotkey(HotkeyConfig config)
    {
        var parts = new System.Collections.Generic.List<string>();
        if (config.Control) parts.Add("Ctrl");
        if (config.Shift) parts.Add("Shift");
        if (config.Alt) parts.Add("Alt");
        parts.Add(((Keys)config.Key).ToString());
        return string.Join("+", parts);
    }

    private void ChangeHotkey()
    {
        using var dialog = new HotkeyCaptureForm(_settings.Hotkey);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _settings.Hotkey = dialog.SelectedHotkey;
            _hotkeyLabel.Text = $"Гаряча клавіша: {FormatHotkey(_settings.Hotkey)}";
        }
    }
}
