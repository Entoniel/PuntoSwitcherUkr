using System;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var context = new TrayApplicationContext();
        Application.Run(context);
    }
}
