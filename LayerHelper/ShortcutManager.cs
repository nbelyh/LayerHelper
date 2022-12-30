using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SciterSharp.Interop;
using Visio = Microsoft.Office.Interop.Visio;

namespace LayerHelper
{
    public class ShortcutManager
    {
        private static HashSet<Keys> KeysFromString(string keys)
        {
            var cult = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            var kc = new KeysConverter();
            var result = new HashSet<Keys>(
                keys
                    .Split(',')
                    .Select(s => s.Trim())
                    .Select(kc.ConvertFromString)
                    .Cast<Keys>()
                    .ToList());

            Thread.CurrentThread.CurrentUICulture = cult;
            return result;
        }

        private static readonly Lazy<HashSet<Keys>> ControlShortcutKeys = new Lazy<HashSet<Keys>>(() =>
            KeysFromString(
                "Tab, Tab+Shift, Tab+Ctrl, Ctrl+Shift+Tab, " +
                "Escape, PgUp, PgDn, End, Home, Left, Up, Right, Down, Ins, " +
                "Del, F3, Shift+PgUp, Shift+PgDn, Shift+End, Shift+Home, Shift+Left, " +
                "Shift+Up, Shift+Right, Shift+Down, Shift+Ins, Shift+Del, Ctrl+Back, Ctrl+Space, Ctrl+End, " +
                "Ctrl+Home, Ctrl+Left, Ctrl+Up, Ctrl+Right, Ctrl+Down, Ctrl+Ins, Ctrl+Del, Ctrl+0, Ctrl+A, " +
                "Ctrl+B, Ctrl+C, Ctrl+E, Ctrl+F, Ctrl+G, Ctrl+H, Ctrl+I, Ctrl+M, Ctrl+N, Ctrl+R, Ctrl+Y, Ctrl+U, " +
                "Ctrl+V, Ctrl+X, Ctrl+Z, Ctrl+Add, Ctrl+Subtract, Ctrl+OemMinus, Ctrl+Shift+End, " +
                "Ctrl+Shift+Home, Ctrl+Shift+Left, Ctrl+Shift+Right, Ctrl+Shift+B, Ctrl+Shift+C, Ctrl+Shift+N, " +
                "Ctrl+Shift+U, Ctrl+Shift+OemMinus, Alt+Back, Alt+Up, Alt+Down, Alt+F, Alt+Shift+Left, " +
                "Alt+Shift+Up, Alt+Shift+Right, Alt+Shift+Down"));

        private static readonly Lazy<HashSet<Keys>> FormShortcutKeys = new Lazy<HashSet<Keys>>(() =>
            KeysFromString("Tab, Tab+Shift"));

        public bool OnKeystrokeMessageForAddon(Visio.MSGWrap msgWrap)
        {
            var keys = (Keys) msgWrap.wParam;
            if ((Control.ModifierKeys & Keys.Control) != 0)
                keys |= Keys.Control;
            if ((Control.ModifierKeys & Keys.Shift) != 0)
                keys |= Keys.Shift;
            
            if (ControlShortcutKeys.Value.Contains(keys))
            {
                var msg = new PInvokeWindows.MSG
                {
                    hwnd = (IntPtr)msgWrap.hwnd,
                    message = (uint)msgWrap.message,
                    wParam = (IntPtr)msgWrap.wParam,
                    lParam = (IntPtr)msgWrap.lParam
                };

                PInvokeWindows.TranslateMessage(ref msg);
                PInvokeWindows.DispatchMessage(ref msg);
                return true;
            }

            return false;
        }
    }
}
