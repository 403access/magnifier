using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Magnifier.HotKeys
{
    public static class HotKeyManager
    {
        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

        private static readonly HashSet<int> RegisteredHotKeys = new HashSet<int>();
        private static bool isInitialized = false;
        private static int currentId = 0;

        static HotKeyManager()
        {
            // Static constructor to ensure one-time initialization
            if (!isInitialized)
            {
                Application.ApplicationExit += OnApplicationExit;
                Application.AddMessageFilter(new HotKeyMessageFilter());
                HotKeyMessageFilter.HotKeyPressed += HandleHotKeyPressed;
                isInitialized = true;
            }
        }

        public static void RegisterHotKey(Form form, Keys key, KeyModifiers modifiers)
        {
            currentId++;
            if (!NativeMethods.RegisterHotKey(form.Handle, currentId, (uint)modifiers, (uint)key))
            {
                throw new InvalidOperationException("Could not register the hotkey.");
            }
            RegisteredHotKeys.Add(currentId);
        }

        public static void UnregisterHotKey(Form form)
        {
            foreach (var id in RegisteredHotKeys)
            {
                NativeMethods.UnregisterHotKey(form.Handle, id);
            }
            RegisteredHotKeys.Clear();
        }

        private static void HandleHotKeyPressed(int id)
        {
            HotKeyPressed?.Invoke(null, new HotKeyEventArgs { HotKeyId = id });
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            // Clean up any remaining hotkeys and handlers
            RegisteredHotKeys.Clear();
            HotKeyMessageFilter.HotKeyPressed -= HandleHotKeyPressed;
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }

    public class HotKeyEventArgs : EventArgs
    {
        public int HotKeyId { get; set; }
    }
}