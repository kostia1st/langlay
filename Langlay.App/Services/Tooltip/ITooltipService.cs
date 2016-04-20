using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Product
{
    public interface ITooltipService
    {
        void Push(string str, Point position, bool resetTimer);
        bool GetIsVisible();
    }
}
