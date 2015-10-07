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

namespace _3DGraphics {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
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
