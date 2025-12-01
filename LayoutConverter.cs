using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuntoSwitcher2;

internal static class LayoutConverter
{
    private static readonly Dictionary<char, char> EnToUa = BuildEnToUa();
    private static readonly Dictionary<char, char> UaToEn = BuildReverse(EnToUa);

    public static void ConvertSelection(LayoutMode mode)
    {
        try
        {
            var backup = Clipboard.GetDataObject();

            SendKeys.SendWait("^c");
            Application.DoEvents();
            Thread.Sleep(50);

            if (!Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                return;
            }

            string original = Clipboard.GetText(TextDataFormat.UnicodeText);
            if (string.IsNullOrWhiteSpace(original))
            {
                return;
            }

            string converted = ConvertText(original, mode == LayoutMode.EnToUa ? EnToUa : UaToEn);

            Clipboard.SetText(converted, TextDataFormat.UnicodeText);
            SendKeys.SendWait("^v");

            if (backup != null)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(80);
                    Clipboard.SetDataObject(backup);
                });
            }
        }
        catch
        {
            // ignore conversion errors to keep the app stable
        }
    }

    private static string ConvertText(string input, Dictionary<char, char> map)
    {
        var sb = new StringBuilder(input.Length);
        foreach (char ch in input)
        {
            bool upper = char.IsUpper(ch);
            char lower = char.ToLowerInvariant(ch);

            if (map.TryGetValue(lower, out char mapped))
            {
                sb.Append(upper ? char.ToUpper(mapped) : mapped);
            }
            else
            {
                sb.Append(ch);
            }
        }

        return sb.ToString();
    }

    private static Dictionary<char, char> BuildEnToUa() => new()
    {
        ['`'] = '§',
        ['q'] = 'й',
        ['w'] = 'ц',
        ['e'] = 'у',
        ['r'] = 'к',
        ['t'] = 'е',
        ['y'] = 'н',
        ['u'] = 'г',
        ['i'] = 'ш',
        ['o'] = 'щ',
        ['p'] = 'з',
        ['['] = 'х',
        [']'] = 'ї',
        ['a'] = 'ф',
        ['s'] = 'і',
        ['d'] = 'в',
        ['f'] = 'а',
        ['g'] = 'п',
        ['h'] = 'р',
        ['j'] = 'о',
        ['k'] = 'л',
        ['l'] = 'д',
        [';'] = 'ж',
        ['\''] = 'є',
        ['z'] = 'я',
        ['x'] = 'ч',
        ['c'] = 'с',
        ['v'] = 'м',
        ['b'] = 'и',
        ['n'] = 'т',
        ['m'] = 'ь',
        [','] = 'б',
        ['.'] = 'ю',
        ['/'] = '.'
    };

    private static Dictionary<char, char> BuildReverse(Dictionary<char, char> source)
    {
        var reversed = new Dictionary<char, char>();
        foreach (var pair in source)
        {
            if (!reversed.ContainsKey(pair.Value))
            {
                reversed[pair.Value] = pair.Key;
            }
        }
        return reversed;
    }
}
