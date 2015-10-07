﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace _3DGraphics {

    public static class ColorExtensionMethods {

        /// <summary>
        /// Blends the specified colors together.
        /// </summary>
        /// <param name="color0">Base color.</param>
        /// <param name="color1">Color onto <paramref name="color0" />.</param>
        /// <param name="ratio1">How much of <paramref name="color1" /> on top of <paramref name="color0" />.</param>
        /// <returns>The blended color.</returns>
        /// <remarks>
        /// [c# - Is there an easy way to blend two System.Drawing.Color values? - Stack Overflow](http://stackoverflow.com/questions/3722307/)
        /// </remarks>
        public static Color Blend(this Color color0, Color color1, double ratio1) {
            var ratio0 = 1 - ratio1;
            return Color.FromArgb(
                (byte)(color0.A * ratio0 + color1.A * ratio1),
                (byte)(color0.R * ratio0 + color1.R * ratio1),
                (byte)(color0.G * ratio0 + color1.G * ratio1),
                (byte)(color0.B * ratio0 + color1.B * ratio1)
            );
        }
    }
}
