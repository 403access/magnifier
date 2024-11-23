using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Magnifier.HotKeys
{

    public class HotKeyMessageFilter : IMessageFilter
    {
        public static event Action<int> HotKeyPressed;

        private static readonly HashSet<int> RegisteredHotKeys = new HashSet<int>();

        private readonly int hotKeyId;

        public HotKeyMessageFilter(int hotKeyId)
        {
            this.hotKeyId = hotKeyId;
            RegisteredHotKeys.Add(hotKeyId);
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (RegisteredHotKeys.Contains(id))
                {
                    HotKeyPressed?.Invoke(id);
                    return true; // Message handled
                }
            }

            return false; // Pass to the next filter
        }
    }
}
