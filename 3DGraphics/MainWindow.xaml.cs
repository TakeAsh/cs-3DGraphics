using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TakeAshUtility;
using WpfUtility;

namespace _3DGraphics {

    using _resources = Properties.Resources;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        public const double AxisRange = 125;
        public const double AxisThickness = 1;
        public static readonly Color AxisColor = Colors.Gray;
        public const double AxisTick = 20;
        public const double TickThickness = 0.5;
        public static readonly Color TickColor = Colors.Cyan;
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

        private static readonly Regex _regVertex = new Regex(@"^Vertex\s+\d+\s+(?<x>[-0-9\.]+)\s+(?<y>[-0-9\.]+)\s+(?<z>[-0-9\.]+)");
        private static readonly Regex _regFace = new Regex(@"^Face\s+\d+\s+(?<p0>[0-9]+)\s+(?<p1>[0-9]+)\s+(?<p2>[0-9]+)");

        private static Properties.Settings _settings = Properties.Settings.Default;

        struct PN {
            public double PX;
            public double PY;
            public double PZ;
            public double NX;
            public double NY;
            public double NZ;

            public PN(double px,double py,double pz)
                : this() {

                    PX = px;
                    PY = py;
                    PZ = pz;
                    NX = px - 50;
                    NY = py;
                    NZ = pz;
            }

            public override string ToString() {
                return String.Join(" ", new[]{
                    PX.ToString("0.000"),
                    PY.ToString("0.000"),
                    PZ.ToString("0.000"),
                    NX.ToString("0.000"),
                    NY.ToString("0.000"),
                    NZ.ToString("0.000"),
                });
            }
        }

        private WindowPlacement _placement;

        public MainWindow() {
            InitializeComponent();
            
            AddAxes(viewport_Gradient);
            AddTicks(viewport_Gradient);
            PrepareGradient(viewport_Gradient);

            AddPoints(viewport_Points, _sphericalPoints);
            var pns = String.Join("\n", _sphericalPoints.Select(point => new PN(point.X, point.Y, point.Z)));
            viewport_Points.Children.Add(new CrossLines3D() {
                Points = _sphericalPoints,
                Color = Colors.Lime,
                Thickness = 3,
            });
            AddMesh(viewport_Points);
            AddAxes(viewport_Points);
            AddTicks(viewport_Points);

            AddAxes(viewport_Frame);
            AddTicks(viewport_Frame);
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

        private CrossLines3D MakeAxis(IEnumerable<Point3D> points) {
            return new CrossLines3D() {
                Points = points,
                Color = AxisColor,
                Thickness = AxisThickness,
            };
        }

        private ModelVisual3D MakeAxisLabel(string label, IEnumerable<Point3D> positions) {
            var label_Axis = new VisualBrush(new TextBlock() { Text = label }).ToMaterial();
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
            Enumerable.Range(1, (int)(AxisRange / AxisTick))
                .Select(i => i * AxisTick)
                .ToList()
                .ForEach(t => {
                    new List<Visual3D>() {
                        MakeTick(new[] {
                            new Point3D(0, t, -AxisRange),
                            new Point3D(0, t, AxisRange),
                        }),
                        MakeTick(new[] {
                            new Point3D(0, -t, -AxisRange),
                            new Point3D(0, -t, AxisRange),
                        }),
                        MakeTick(new[] {
                            new Point3D(0, -AxisRange, t),
                            new Point3D(0, AxisRange, t),
                        }),
                        MakeTick(new[] {
                            new Point3D(0, -AxisRange, -t),
                            new Point3D(0, AxisRange, -t),
                        }),
                    }.ForEach(item => viewport3D.Children.Add(item));
                });
        }

        private CrossLines3D MakeTick(IEnumerable<Point3D> points) {
            return new CrossLines3D() {
                Points = points,
                Color = TickColor,
                Thickness = TickThickness,
            };
        }

        private void AddPoints(Viewport3D viewport3D, List<Point3D> points) {
            var materials = new List<Material>() {
                Color.FromArgb(0xff, 0xff, 0x00, 0x00).ToMaterial(),
                Color.FromArgb(0xff, 0xff, 0xff, 0x00).ToMaterial(),
                Color.FromArgb(0xff, 0x00, 0xff, 0x00).ToMaterial(),
                Color.FromArgb(0xff, 0x00, 0xff, 0xff).ToMaterial(),
                Color.FromArgb(0xff, 0x00, 0x00, 0xff).ToMaterial(),
                Color.FromArgb(0xff, 0xff, 0x00, 0xff).ToMaterial(),
            };
            viewport3D.Children.Add(new ModelVisual3D() {
                Content = points.ToOctaHedrons(materials, 1, new TranslateTransform3D(-50, 0, 0)),
            });
        }

        private void AddMesh(Viewport3D viewport3D) {
            var mesh = _resources.Mesh01;
            var points = new Point3DCollection();
            var indicies = new Int32Collection();
            mesh.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(line => {
                    MatchCollection mc;
                    if (line[0] == '#') {
                        return;
                    } else if ((mc = _regVertex.Matches(line)).Count > 0) {
                        points.Add(new Point3D(
                            Convert.ToDouble(mc[0].Groups["x"].Value),
                            Convert.ToDouble(mc[0].Groups["y"].Value),
                            Convert.ToDouble(mc[0].Groups["z"].Value)
                        ));
                    } else if ((mc = _regFace.Matches(line)).Count > 0) {
                        new List<int>(){
                            Convert.ToInt32(mc[0].Groups["p0"].Value) - 1,
                            Convert.ToInt32(mc[0].Groups["p1"].Value) - 1,
                            Convert.ToInt32(mc[0].Groups["p2"].Value) - 1
                        }.ForEach(point => indicies.Add(point));
                    }
                });
            viewport3D.Children.Add(new ModelVisual3D() {
                Content = new GeometryModel3D() {
                    Geometry = new MeshGeometry3D() {
                        Positions = points,
                        TriangleIndices = indicies,
                    },
                    Material = Color.FromArgb(0x7f, 0xff, 0x00, 0x00).ToMaterial(),
                },
            });
        }

        private void PrepareGradient(Viewport3D viewport3D) {
            var angleA = new Point3D(0, 80, 20);
            var angleB = new Point3D(0, 80, 80);
            var angleC = new Point3D(0, 20, 20);
            var angleD = new Point3D(0, 20, 80);
            var sideDE = new[]{
                new Point3D(5, 20, 70),
                new Point3D(5, 20, 60),
                new Point3D(0, 20, 50),
                new Point3D(-5, 20, 40),
                new Point3D(-5, 20, 30),
                new Point3D(0, 20, 20),
            };
            var sideDF = new[]{
                new Point3D(-5, 30, 80),
                new Point3D(-5, 40, 80),
                new Point3D(0, 50, 80),
                new Point3D(5, 60, 80),
                new Point3D(5, 70, 80),
                new Point3D(0, 80, 80),
            };
            var material1 = (TryFindResource("texture_BkYR") as Brush).ToMaterial();
            var material2 = (TryFindResource("texture_WRY") as Brush).ToMaterial();
            var visual = new[] {
                Triangle3D.Create(angleA, angleB, angleC, material1, true),
                Triangle3D.Create(angleD, sideDE, sideDF, material2, true),
            }.ToModelVisual3D();
            viewport3D.Children.Add(visual);
        }

        #region Event Handlers

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            _placement = new WindowPlacement(this) {
                Placement = _settings.WindowPlacement,
            };
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if (!e.Cancel) {
                _settings.WindowPlacement = _placement.Placement;
                _settings.Save();
            }
        }

        private void button_Camera_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var name = button.Name.Substring(button.Name.LastIndexOf('_') + 1);
            PerspectiveCamera camera = null;
            switch (name) {
                case "XpYpZp":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(200, 200, 200),
                        LookDirection = new Vector3D(-1, -1, -1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XpYmZp":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(200, -200, 200),
                        LookDirection = new Vector3D(-1, 1, -1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XpYmZm":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(200, -200, -200),
                        LookDirection = new Vector3D(-1, 1, 1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XpYpZm":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(200, 200, -200),
                        LookDirection = new Vector3D(-1, -1, 1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XmYpZp":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(-200, 200, 200),
                        LookDirection = new Vector3D(1, -1, -1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XmYmZp":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(-200, -200, 200),
                        LookDirection = new Vector3D(1, 1, -1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XmYmZm":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(-200, -200, -200),
                        LookDirection = new Vector3D(1, 1, 1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
                case "XmYpZm":
                    camera = new PerspectiveCamera() {
                        Position = new Point3D(-200, 200, -200),
                        LookDirection = new Vector3D(1, -1, 1),
                        UpDirection = new Vector3D(1, 0, 0),
                    };
                    break;
            }
            viewport_Gradient.Camera = camera;
            viewport_Frame.Camera = camera;
        }

        #endregion
    }
}
