using System.Drawing;

namespace Product
{
    public interface ITooltipService
    {
        void Push(string str, Point position, bool resetTimer);

        string GetDisplayString();

        bool GetIsVisible();
    }
}