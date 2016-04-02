using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Product
{
    public class AppMessageFilter: IMessageFilter
    {
        public Action OnClose;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == SafeMethods.WM_CLOSE
                || m.Msg == SafeMethods.WM_DESTROY
                || m.Msg == SafeMethods.WM_QUIT)
            {
                if (OnClose != null)
                    OnClose();
            }
            return false;
        }
    }
}
