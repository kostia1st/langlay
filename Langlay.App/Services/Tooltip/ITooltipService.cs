using System.Drawing;

namespace Product {
    public interface ITooltipService {
        string GetDisplayString();
        bool GetIsVisible();
        void Push(string str, Point position, bool resetTimer);
    }
}