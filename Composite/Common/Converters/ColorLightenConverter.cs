using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Composite.Common.Converters
{
    public class ColorLightenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            double lightenFactor = 0.1;
            if (parameter != null && double.TryParse(parameter.ToString(), out double param)) lightenFactor = param;

            try
            {
                Color color;
                if (value is string colorString)
                {
                    if (string.IsNullOrWhiteSpace(colorString)) return null;
                    if (colorString == "White") return new SolidColorBrush(Color.FromRgb(230, 230, 230));
                    else
                    {
                        try { color = (Color)ColorConverter.ConvertFromString(colorString); }
                        catch { return null; }
                    }
                }

                return new SolidColorBrush(LightenColor(color, lightenFactor));
            }
            catch { return null; }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        Color LightenColor(Color color, double factor)
        {
            var hsl = RgbToHsl(color.R, color.G, color.B);
            hsl.L = Math.Min(1.0, hsl.L + factor);
            var rgb = HslToRgb(hsl.H, hsl.S, hsl.L);

            return Color.FromArgb(color.A, (byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }
        (double H, double S, double L) RgbToHsl(byte r, byte g, byte b)
        {
            double rd = r / 255.0;
            double gd = g / 255.0;
            double bd = b / 255.0;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double delta = max - min;

            double h = 0;
            double s = 0;
            double l = (max + min) / 2.0;

            if (delta != 0)
            {
                s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

                if (max == rd) h = (gd - bd) / delta + (gd < bd ? 6 : 0);
                else if (max == gd) h = (bd - rd) / delta + 2;
                else if (max == bd) h = (rd - gd) / delta + 4;

                h /= 6;
            }

            return (h, s, l);
        }
        (int R, int G, int B) HslToRgb(double h, double s, double l)
        {
            double r, g, b;

            if (s == 0) r = g = b = l;
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;
                r = HueToRgb(p, q, h + 1.0 / 3.0);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0 / 3.0);
            }

            return ((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }
        double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
            return p;
        }
    }
}