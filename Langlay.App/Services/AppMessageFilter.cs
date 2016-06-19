using System;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class AppMessageFilter : IMessageFilter
    {
        public Action OnClose;

        public bool PreFilterMessage(ref Message m)
        {
            var result = false;
            if (m.Msg.In(Win32.WM_DESTROY, Win32.WM_QUIT, Win32.WM_USER_QUIT))
            {
                OnClose?.Invoke();
                result = true;
            }
            return result;
        }
    }
}