using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _3DTools;

namespace _3DGraphics {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        public const double AxisRange = 120;
        public const double AxisThickness = 3;
        public const double AxisTick = 20;
        public const double TickThickness = 1;
        public const double LabelSize = 10;

        private static readonly Int32Collection _triangleIndices = new Int32Collection() {
            0, 1, 2,
            2, 3, 0,
        };

        private static readonly PointCollection _textureCoordinates = new PointCollection() {
            new Point(0, 0),
            new Point(0, 1),
            new Point(1, 1),
            new Point(1, 0),
        };

        public MainWindow() {
            InitializeComponent();
            AddAxes(viewport_Gradient);
            AddTicks(viewport_Gradient);
        }

        private void AddAxes(Viewport3D viewport3D) {
            new List<Visual3D>(){
                MakeAxis(new[] {
                    new Point3D(0, 0, 0),
                    new Point3D(AxisRange, 0, 0),
                }),
                MakeAxis(new[] {
                    new Point3D(0, -AxisRange, 0),
                    new Point3D(0, AxisRange, 0),
                }),
                MakeAxis(new[] {
                    new Point3D(0, 0, -AxisRange),
                    new Point3D(0, 0, AxisRange),
                }),
                MakeAxisLabel(
                    "+x",
                    new[] {
                        new Point3D(AxisRange, 0, 0),
                        new Point3D(AxisRange - LabelSize, 0, 0),
                        new Point3D(AxisRange - LabelSize, 0, LabelSize),
                        new Point3D(AxisRange, 0, LabelSize),
                    }
                ),
                MakeAxisLabel(
                    "+y",
                    new[] {
                        new Point3D(0, AxisRange, 0),
                        new Point3D(0, AxisRange - LabelSize, 0),
                        new Point3D(LabelSize, AxisRange - LabelSize, 0),
                        new Point3D(LabelSize, AxisRange, 0),
                    }
                ),
                MakeAxisLabel(
                    "-y",
                    new[] {
                        new Point3D(0, -AxisRange, 0),
                        new Point3D(0, -AxisRange + LabelSize, 0),
                        new Point3D(-LabelSize, -AxisRange + LabelSize, 0),
                        new Point3D(-LabelSize, -AxisRange, 0),
                    }
                ),
                MakeAxisLabel(
                    "+z",
                    new[] {
                        new Point3D(0, 0, AxisRange),
                        new Point3D(0, 0, AxisRange - LabelSize),
                        new Point3D(0, LabelSize, AxisRange - LabelSize),
                        new Point3D(0, LabelSize, AxisRange),
                    }
                ),
                MakeAxisLabel(
                    "-z",
                    new[] {
                        new Point3D(0, 0, -AxisRange),
                        new Point3D(0, 0, -AxisRange + LabelSize),
                        new Point3D(0, -LabelSize, -AxisRange + LabelSize),
                        new Point3D(0, -LabelSize, -AxisRange),
                    }
                ),
            }.ForEach(item => viewport3D.Children.Add(item));
        }

        private ScreenSpaceLines3D MakeAxis(IEnumerable<Point3D> points) {
            return new ScreenSpaceLines3D() {
                Points = new Point3DCollection(points),
                Color = Colors.Gray,
                Thickness = AxisThickness,
            };
        }

        private ModelVisual3D MakeAxisLabel(string label, IEnumerable<Point3D> positions) {
            var label_Axis = new DiffuseMaterial(new VisualBrush(new TextBlock() { Text = label }));
            return new ModelVisual3D() {
                Content = new GeometryModel3D() {
                    Material = label_Axis,
                    BackMaterial = label_Axis,
                    Geometry = new MeshGeometry3D() {
                        Positions = new Point3DCollection(positions),
                        TriangleIndices = _triangleIndices,
                        TextureCoordinates = _textureCoordinates,
                    },
                },
            };
        }

        private void AddTicks(Viewport3D viewport3D) {
            Enumerable.Range(1, (int)(AxisRange * 2 / AxisTick) - 1)
                .Select(i => i * AxisTick - AxisRange)
                .ToList()
                .ForEach(t => {
                    new List<Visual3D>() {
                        new ScreenSpaceLines3D() {
                            Points = new Point3DCollection(){
                                new Point3D(0, t, -AxisRange),
                                new Point3D(0, t, AxisRange),
                            },
                            Color = Colors.LightGray,
                            Thickness = TickThickness,
                        },
                        new ScreenSpaceLines3D() {
                            Points = new Point3DCollection(){
                                new Point3D(0, -AxisRange, t),
                                new Point3D(0, AxisRange, t),
                            },
                            Color = Colors.LightGray,
                            Thickness = TickThickness,
                        },
                    }.ForEach(item => viewport3D.Children.Add(item));
                });
        }

        private void button_XpYpZp_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(200, 200, 200),
                LookDirection = new Vector3D(-1, -1, -1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XpYmZp_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(200, -200, 200),
                LookDirection = new Vector3D(-1, 1, -1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XpYmZm_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(200, -200, -200),
                LookDirection = new Vector3D(-1, 1, 1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XpYpZm_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(200, 200, -200),
                LookDirection = new Vector3D(-1, -1, 1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XmYpZp_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(-200, 200, 200),
                LookDirection = new Vector3D(1, -1, -1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XmYmZp_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(-200, -200, 200),
                LookDirection = new Vector3D(1, 1, -1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XmYmZm_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(-200, -200, -200),
                LookDirection = new Vector3D(1, 1, 1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }

        private void button_XmYpZm_Click(object sender, RoutedEventArgs e) {
            viewport_Gradient.Camera = new PerspectiveCamera() {
                Position = new Point3D(-200, 200, -200),
                LookDirection = new Vector3D(1, -1, 1),
                UpDirection = new Vector3D(1, 0, 0),
            };
        }
    }
}
