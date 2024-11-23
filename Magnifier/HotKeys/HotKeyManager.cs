using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Magnifier.HotKeys
{
    public static class HotKeyManager
    {
        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

        private static int currentId = 0;

        public static void RegisterHotKey(Form form, Keys key, KeyModifiers modifiers)
        {
            currentId++;
            if (!RegisterHotKey(form.Handle, currentId, (uint)modifiers, (uint)key))
            {
                throw new InvalidOperationException("Could not register the hotkey.");
            }

            Application.AddMessageFilter(new HotKeyMessageFilter(currentId));
            HotKeyMessageFilter.HotKeyPressed += OnHotKeyPressed;
        }

        public static void UnregisterHotKey(Form form)
        {
            UnregisterHotKey(form.Handle, currentId);
        }

        private static void OnHotKeyPressed(int id)
        {
            HotKeyPressed?.Invoke(null, new HotKeyEventArgs { HotKeyId = id });
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    public class HotKeyEventArgs : EventArgs
    {
        public int HotKeyId { get; set; }
    }
}