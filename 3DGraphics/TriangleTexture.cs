using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;

namespace _3DGraphics {

    public static class TriangleTexture {

        public const int DefaultSize = 8;

        public static BitmapSource Create(Color c0, Color c1, Color c2, int size = DefaultSize) {
            var format = PixelFormats.Bgra32;
            var bytesPerPixel = (format.BitsPerPixel + 7) / 8;
            var rawStride = size * bytesPerPixel;
            var rawImage = new byte[rawStride * size];
            for (var v = 0; v < size; ++v) {
                for (var u = 0; u <= size - v; ++u) {
                    var uv = u + v;
                    var c = c1.Blend(c2, (double)u / uv).Blend(c0, (double)uv / size);
                    rawImage[u * bytesPerPixel + 0 + v * rawStride] = c.B;
                    rawImage[u * bytesPerPixel + 1 + v * rawStride] = c.G;
                    rawImage[u * bytesPerPixel + 2 + v * rawStride] = c.R;
                    rawImage[u * bytesPerPixel + 3 + v * rawStride] = c.A;
                }
            }
            return BitmapSource.Create(size, size, 96, 96, format, null, rawImage, rawStride);
        }
    }

    [MarkupExtensionReturnType(typeof(BitmapSource))]
    public class TriangleTextureExtension :
        MarkupExtension {

        public const string TriangleTextureSizeKey = "TriangleTexture_Size";

        private Color _c0;
        private Color _c1;
        private Color _c2;
        private int _size = TriangleTexture.DefaultSize;

        public int Size {
            get { return _size; }
            set { _size = value; }
        }

        public TriangleTextureExtension(string c0, string c1, string c2) {
            _c0 = (Color)ColorConverter.ConvertFromString(c0);
            _c1 = (Color)ColorConverter.ConvertFromString(c1);
            _c2 = (Color)ColorConverter.ConvertFromString(c2);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (Size == TriangleTexture.DefaultSize &&
                serviceProvider.GetService<IXamlSchemaContextProvider>() != null) {
                try {
                    var sizeStatic = new StaticResourceExtension(TriangleTextureSizeKey);
                    Size = (int)sizeStatic.ProvideValue(serviceProvider);
                }
                catch {
                    // When StaticResource 'TriangleTexture_Size' is not defined, an exception will be thrown.
                    // Cannot find resource named 'TriangleTexture_Size'. Resource names are case sensitive.
                    // This exception is ignored, and Size is not changed.
                }
            }
            var sizeDynamic = Application.Current.TryFindResource(TriangleTextureSizeKey);
            if (Size == TriangleTexture.DefaultSize &&
                sizeDynamic != null) {
                Size = (int)sizeDynamic;
            }
            return TriangleTexture.Create(_c0, _c1, _c2, Size);
        }
    }
}
