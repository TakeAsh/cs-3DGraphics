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
using MediaHelper;

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
        public const double OctaHedronRadius = 1;
        public const int DivYZ = 24;
        public const int NumOfPoints = 24 * DivYZ;

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

        private static readonly List<int> _octahedronIndices = new List<int>(){
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            5, 2, 1,
            5, 1, 4,
            5, 4, 3,
            5, 3, 2,
        };

        private static readonly List<Point3D> _sphericalPoints =
            new List<Point3D>(Enumerable
                .Range(0, NumOfPoints)
                .Select(t => {
                    var rYZ = Math.Sin(Math.PI * t / NumOfPoints) * 50;
                    return new Point3D(
                        Math.Cos(Math.PI * t / NumOfPoints) * 50 + 50,
                        rYZ * Math.Cos(Math.PI * 2 * (t % DivYZ) / DivYZ),
                        rYZ * Math.Sin(Math.PI * 2 * (t % DivYZ) / DivYZ)
                    );
                }));

        public MainWindow() {
            InitializeComponent();
            
            AddAxes(viewport_Gradient);
            AddTicks(viewport_Gradient);

            AddPoints(viewport_Points, _sphericalPoints);
            AddAxes(viewport_Points);
            AddTicks(viewport_Points);
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

        private void AddPoints(Viewport3D viewport3D, List<Point3D> points) {
            var points2 = new Point3DCollection();
            var indicies2 = new Int32Collection();
            var index = 0;
            points.ForEach(point => {
                new List<Point3D>() {
                    new Point3D(point.X + OctaHedronRadius, point.Y, point.Z),
                    new Point3D(point.X, point.Y + OctaHedronRadius, point.Z),
                    new Point3D(point.X, point.Y, point.Z + OctaHedronRadius),
                    new Point3D(point.X, point.Y - OctaHedronRadius, point.Z),
                    new Point3D(point.X, point.Y, point.Z - OctaHedronRadius),
                    new Point3D(point.X - OctaHedronRadius, point.Y, point.Z),
                }.ForEach(point2 => points2.Add(point2));
                _octahedronIndices.ForEach(pos => indicies2.Add(pos + index * 6));
                ++index;
            });
            viewport3D.Children.Add(new ModelVisual3D() {
                Content = new GeometryModel3D() {
                    Geometry = new MeshGeometry3D() {
                        Positions = points2,
                        TriangleIndices = indicies2,
                    },
                    Material = new DiffuseMaterial(Brushes.Red),
                },
            });
        }

        #region Event Handlers

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

        #endregion
    }
}
