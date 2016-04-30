using System;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class AppMessageFilter: IMessageFilter
    {
        public Action OnClose;
        public Action OnRestart;
        public bool PreFilterMessage(ref Message m)
        {
            var result = false;
            if (m.Msg == Win32.WM_CLOSE
                || m.Msg == Win32.WM_DESTROY
                || m.Msg == Win32.WM_QUIT)
            {
                if (OnClose != null)
                    OnClose();
            }
            if (m.Msg == Win32.WM_USER_RESTART)
            {
                if (OnRestart != null)
                    OnRestart();
                result = true;
            }
            return result;
        }
    }
}
