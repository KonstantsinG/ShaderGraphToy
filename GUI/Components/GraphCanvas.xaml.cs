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
        // dragging Canvas params
        private bool _draggingCanvas = false;
        private Point _canvasDraggingOffset;

        // Canvas zoom params
        private double _zoomRate = 1.1;
        private double _minZoom = 0.4;
        private double _maxZoom = 2.0;
        
        // Canvas markup params
        private int _gridStep = 60;
        private Point _gridOffset = new Point(30, 30);
        private int _dotSize = 4;
        private List<Ellipse> _gridDots = new List<Ellipse>();

        // viewport resize params
        private double _prevViewportWidth;
        private double _prevViewportHeight;


        public GraphCanvas()
        {
            InitializeComponent();
            TranslateCanvas(-2000);
            _prevViewportWidth = ((Grid)mainCanvas.Parent).ActualWidth;
            _prevViewportHeight = ((Grid)mainCanvas.Parent).ActualHeight;
        }

        private void TranslateCanvas(double val)
        {
            Matrix matr = matrixTransform.Matrix;
            matr.Translate(val, val);
            matrixTransform.Matrix = matr;
        }


        private void ClearCanvasGrid()
        {
            for (int i = 0; i < _gridDots.Count; i++)
                mainCanvas.Children.Remove(_gridDots[i]);

            _gridDots.Clear();
        }

        private void DrawMarkup()
        {
            ClearCanvasGrid();

            for (int px = (int)_gridOffset.X; px < mainCanvas.ActualWidth; px += _gridStep)
            {
                for (int py = (int)_gridOffset.Y; py < mainCanvas.ActualHeight; py += _gridStep)
                {
                    Ellipse el = new Ellipse();
                    if ((double)(px / _gridStep) % 5 == 0 && (double)(py / _gridStep) % 5 == 0) // highlight every 5'th dot
                    {
                        el.Height = _dotSize + 1;
                        el.Width = _dotSize + 1;
                        el.Fill = (Brush)FindResource("Gray_04");
                    }
                    else // regular dot
                    {
                        el.Height = _dotSize;
                        el.Width = _dotSize;
                        el.Fill = (Brush)FindResource("Gray_03");
                    }

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
            Matrix matrix = matrixTransform.Matrix;
            double scale = e.Delta > 0 ? _zoomRate : 1 / _zoomRate;
            double newZoom = matrix.M11 * scale;

            if (newZoom < _minZoom || newZoom > _maxZoom)
                return;

            Point mousePosition = e.GetPosition(mainCanvas);
            matrix.ScaleAtPrepend(scale, scale, mousePosition.X, mousePosition.Y);

            // Canvas viewport sizes
            double trueVpSizeX = ((Grid)mainCanvas.Parent).ActualWidth / matrix.M11;
            double trueVpSizeY = ((Grid)mainCanvas.Parent).ActualHeight / matrix.M22;

            // Canvas part before viewport
            double trueOffsetX = -matrix.OffsetX / matrix.M11;
            double trueOffsetY = -matrix.OffsetY / matrix.M22;

            // Canvas part after viewport (reversed)
            double overflowX = trueVpSizeX + trueOffsetX - mainCanvas.ActualWidth;
            double overflowY = trueVpSizeY + trueOffsetY - mainCanvas.ActualHeight;

            Point offsetPoint = new Point(0, 0);

            if (matrix.OffsetX > 0) // left side offset
                offsetPoint.X += -matrix.OffsetX;

            if (matrix.OffsetY > 0) // top side offset
                offsetPoint.Y += -matrix.OffsetY;

            if (overflowX > 0) // right side offset
                offsetPoint.X += overflowX;

            if (overflowY > 0) // bottom side offset
                offsetPoint.Y += overflowY;

            matrix.Translate(offsetPoint.X, offsetPoint.Y);
            matrixTransform.Matrix = matrix;
        }

        private void mainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _canvasDraggingOffset = e.GetPosition(this);
                _draggingCanvas = true;
                mainCanvas.CaptureMouse();
            }
        }

        private void mainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _draggingCanvas = false;
                mainCanvas.ReleaseMouseCapture();
            }
        }

        private void mainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingCanvas)
            {
                Point mousePos = e.GetPosition(this);
                Matrix matrix = matrixTransform.Matrix;

                double translateValueX = mousePos.X - _canvasDraggingOffset.X;
                double translateValueY = mousePos.Y - _canvasDraggingOffset.Y;
                bool translateX = CheckTranslationX(translateValueX);
                bool translateY = CheckTranslationY(translateValueY);

                if (translateX) matrix.Translate(translateValueX, 0);
                if (translateY) matrix.Translate(0, translateValueY);

                _canvasDraggingOffset = mousePos;
                matrixTransform.Matrix = matrix;
            }
        }

        private bool CheckTranslationX(double translateValueX)
        {
            Matrix tstMatrix = matrixTransform.Matrix;
            tstMatrix.Translate(translateValueX, 0);

            double trueVpSizeX = ((Grid)mainCanvas.Parent).ActualWidth / tstMatrix.M11;
            double trueOffsetX = -tstMatrix.OffsetX / tstMatrix.M11;

            if (tstMatrix.OffsetX > 0 || trueOffsetX + trueVpSizeX > mainCanvas.ActualWidth)
                return false;
            else
                return true;
        }

        private bool CheckTranslationY(double translateValueY)
        {
            Matrix tstMatrix = matrixTransform.Matrix;
            tstMatrix.Translate(0, translateValueY);

            double trueVpSizeY = ((Grid)mainCanvas.Parent).ActualHeight / tstMatrix.M22;
            double trueOffsetY = -tstMatrix.OffsetY / tstMatrix.M22;

            if (tstMatrix.OffsetY > 0 || trueOffsetY + trueVpSizeY > mainCanvas.ActualHeight)
                return false;
            else
                return true;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newViewportWidth = ((Grid)mainCanvas.Parent).ActualWidth;
            double newViewportHeight = ((Grid)mainCanvas.Parent).ActualHeight;
            double translateValueX = newViewportWidth - _prevViewportWidth;
            double translateValueY = newViewportHeight - _prevViewportHeight;

            Matrix matrix = matrixTransform.Matrix;
            if (CheckTranslationX(translateValueX))
                matrix.Translate(translateValueX, 0);
            if (CheckTranslationY(translateValueY))
                matrix.Translate(0, translateValueY);
            matrixTransform.Matrix = matrix;

            _prevViewportWidth = newViewportWidth;
            _prevViewportHeight = newViewportHeight;
        }
    }
}
