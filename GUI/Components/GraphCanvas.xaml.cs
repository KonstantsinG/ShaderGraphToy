using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для GraphCanvas.xaml
    /// </summary>
    public partial class GraphCanvas : UserControl
    {
        private double _currentZoom = 1.0;
        private double _zoomRate = 1.1;
        private double _minZoom = 0.5;
        private double _maxZoom = 2.0;

        private double _currentTranslateX = 0.0;
        private double _currentTranslateY = 0.0;

        private int _gridStep = 50;
        private Point _gridOffset = new Point(20, 20);
        private int _dotSize = 3;
        private List<Ellipse> _gridDots = new List<Ellipse>();


        public GraphCanvas()
        {
            InitializeComponent();
        }


        private void ClearCanvasGrid()
        {
            for (int i = 0; i < _gridDots.Count; i++)
            {
                mainCanvas.Children.Remove(_gridDots[i]);
            }

            _gridDots.Clear();
        }

        private void DrawMarkup()
        {
            ClearCanvasGrid();

            for (int px = (int)_gridOffset.X; px < mainCanvas.ActualWidth; px += _gridStep)
            {
                for (int py = (int)_gridOffset.Y; py < mainCanvas.ActualHeight; py += _gridStep)
                {
                    Ellipse el = new Ellipse() { Height = _dotSize, Width = _dotSize, Fill = (Brush)FindResource("Gray_03") };
                    Panel.SetZIndex(el, -4);
                    Canvas.SetLeft(el, px);
                    Canvas.SetTop(el, py);
                    _gridDots.Add(el);
                    mainCanvas.Children.Add(el);
                }
            }
        }

        private void mainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMarkup();
        }

        private void mainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            double zoomDelta = e.Delta > 0 ? _zoomRate : 1 / _zoomRate;
            double newZoom = _currentZoom * zoomDelta;

            if (newZoom < _minZoom || newZoom > _maxZoom)
                return;

            Point mousePos = e.GetPosition(mainCanvas);

            scaleTransform.ScaleX = newZoom;
            scaleTransform.ScaleY = newZoom;

            Vector delta = new Vector(mousePos.X * (1 - zoomDelta), mousePos.Y * (1 - zoomDelta));
            _currentTranslateX += _currentZoom * delta.X;
            _currentTranslateY += _currentZoom * delta.Y;

            scrollViewer.ScrollToHorizontalOffset(_currentTranslateX + (scrollViewer.ActualWidth / 2));
            scrollViewer.ScrollToVerticalOffset(_currentTranslateY + (scrollViewer.ActualHeight / 2));

            //translateTransform.X = _currentTranslateX;
            //translateTransform.Y = _currentTranslateY;

            _currentZoom = newZoom;
        }
    }
}
