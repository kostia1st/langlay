using System.Windows.Media;

namespace Product.Common {
    public static class ColorUtils {
        public static System.Drawing.Color ToWinForms(this Color color) {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public static Color ToWpf(this System.Drawing.Color color) {
            return Color.FromRgb(color.R, color.G, color.B);
        }

        public static string ToHtml(this Color color) {
            return System.Drawing.ColorTranslator.ToHtml(color.ToWinForms());
        }

        public static Color FromHtml(this string color) {
            return System.Drawing.ColorTranslator.FromHtml(color).ToWpf();
        }

        public static System.Drawing.Color ApplyAlpha(this System.Drawing.Color color, byte alpha) {
            return System.Drawing.Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}
