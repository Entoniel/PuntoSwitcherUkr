using System;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal sealed class HotkeyCaptureForm : Form
{
    private readonly Label _infoLabel;
    public HotkeyConfig SelectedHotkey { get; private set; }

    public HotkeyCaptureForm(HotkeyConfig current)
    {
        SelectedHotkey = new HotkeyConfig
        {
            Control = current.Control,
            Alt = current.Alt,
            Shift = current.Shift,
            Key = current.Key
        };

        Text = "Нова гаряча клавіша";
        Width = 360;
        Height = 140;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;

        _infoLabel = new Label
        {
            Text = "Натисніть бажану комбінацію (Esc — скасувати)",
            Dock = DockStyle.Fill,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        Controls.Add(_infoLabel);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
        {
            return; // модифікатор без основної клавіші
        }

        SelectedHotkey = new HotkeyConfig
        {
            Control = e.Control,
            Shift = e.Shift,
            Alt = e.Alt,
            Key = (int)e.KeyCode
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
