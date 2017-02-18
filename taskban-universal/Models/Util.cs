using Windows.UI;
using Windows.UI.Xaml.Media;
using TamedTasks.Models.Common;

namespace TamedTasks.Models
{
    public static class Util
    {
        public static byte[] ConvertBrushToByteArray(SolidColorBrush brush)
        {
            var color = brush.Color;
            var c = new byte[4];
            c[0] = color.A;
            c[1] = color.R;
            c[2] = color.G;
            c[3] = color.B;
            return c;
        }

        public static SolidColorBrush GetBoardBrush(Board board)
        {
            SolidColorBrush brush;
            var color = board?.Color;
            if (color == null || color.Length != 4) // color is stored as argb byte array
            {
                brush = new SolidColorBrush(Color.FromArgb(255, 38, 166, 154));
            }
            else
            {
                brush = new SolidColorBrush(Color.FromArgb(color[0], color[1], color[2], color[3]));
            }

            return brush;
        }
    }
}
