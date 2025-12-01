using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace PuntoSwitcher2;

internal static class IconFactory
{
    public static Icon CreateKeyboardSwapIcon()
    {
        int size = 32;
        using var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            var keyboardRect = new Rectangle(4, 14, 24, 12);
            using var keyboardBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            using var keyboardPen = new Pen(Color.FromArgb(90, 90, 90), 1.5f);
            g.FillRectangle(keyboardBrush, keyboardRect);
            g.DrawRectangle(keyboardPen, keyboardRect);

            // keys
            for (int row = 0; row < 2; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    var keyRect = new Rectangle(6 + col * 4, 16 + row * 5, 3, 3);
                    using var keyBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
                    g.FillRectangle(keyBrush, keyRect);
                }
            }

            // double arrows above keyboard
            using var arrowPen = new Pen(Color.FromArgb(60, 120, 210), 2)
            {
                EndCap = LineCap.ArrowAnchor,
                StartCap = LineCap.Round
            };
            g.DrawLine(arrowPen, new Point(10, 10), new Point(16, 6));
            g.DrawLine(arrowPen, new Point(22, 10), new Point(16, 6));

            using var arrowPenDown = new Pen(Color.FromArgb(60, 120, 210), 2)
            {
                EndCap = LineCap.ArrowAnchor,
                StartCap = LineCap.Round
            };
            g.DrawLine(arrowPenDown, new Point(10, 6), new Point(16, 10));
            g.DrawLine(arrowPenDown, new Point(22, 6), new Point(16, 10));
        }

        // clone icon to avoid disposing handle with bitmap
        var handle = bmp.GetHicon();
        var icon = (Icon)Icon.FromHandle(handle).Clone();
        DestroyIcon(handle);
        return icon;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);
}
