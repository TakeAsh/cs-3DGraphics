using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace _3DGraphics {

    public static class ColorExtensionMethods {

        /// <summary>
        /// Blends the specified colors together.
        /// </summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep, "on top of" <paramref name="backColor"/>.</param>
        /// <returns>The blended color.</returns>
        /// <remarks>
        /// [c# - Is there an easy way to blend two System.Drawing.Color values? - Stack Overflow](http://stackoverflow.com/questions/3722307/)
        /// </remarks>
        public static Color Blend(this Color color, Color backColor, double amount) {
            var r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            var g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            var b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            var a = (byte)((color.A * amount) + backColor.A * (1 - amount));
            return Color.FromArgb(a, r, g, b);
        }
    }
}
