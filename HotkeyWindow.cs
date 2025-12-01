using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal sealed class HotkeyWindow : Form
{
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 1;
    private const int MOD_ALT = 0x1;
    private const int MOD_CONTROL = 0x2;
    private const int MOD_SHIFT = 0x4;

    private HotkeyConfig _hotkey = HotkeyConfig.Default();
    private bool _hotkeyRegistered;

    public LayoutMode LayoutMode { get; set; } = LayoutMode.EnToUa;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Visible = false;
        ShowInTaskbar = false;
        Opacity = 0;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TryRegisterHotkey();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        UnregisterHotKey(Handle, HOTKEY_ID);
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            LayoutConverter.ConvertSelection(LayoutMode);
        }

        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        UnregisterHotKey(Handle, HOTKEY_ID);
        base.OnFormClosing(e);
    }

    public void UpdateHotkey(HotkeyConfig config)
    {
        _hotkey = config;
        TryRegisterHotkey();
    }

    private void TryRegisterHotkey()
    {
        if (Handle == IntPtr.Zero)
        {
            CreateHandle();
        }

        UnregisterHotKey(Handle, HOTKEY_ID);

        int modifiers = 0;
        if (_hotkey.Control) modifiers |= MOD_CONTROL;
        if (_hotkey.Alt) modifiers |= MOD_ALT;
        if (_hotkey.Shift) modifiers |= MOD_SHIFT;

        bool registered = RegisterHotKey(Handle, HOTKEY_ID, modifiers, _hotkey.Key);
        _hotkeyRegistered = registered;
        if (!registered)
        {
            MessageBox.Show("Не вдалося зареєструвати обрану гарячу клавішу. Спробуйте іншу комбінацію.",
                "PuntoSwitcherUkr", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
