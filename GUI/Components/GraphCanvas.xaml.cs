using GUI.Controls;
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
        private const double ZOOM_RATE = 1.1;
        private const double MIN_ZOOM = 0.4;
        private const double MAX_ZOOM = 2.0;
        
        // Canvas markup params
        private const int GRID_STEP = 60;
        private Point _gridOffset = new(30, 30);
        private const int DOT_SIZE = 4;
        private readonly List<Ellipse> _gridDots = [];

        // viewport resize params
        private double _prevViewportWidth;
        private double _prevViewportHeight;


        public GraphCanvas()
        {
            InitializeComponent();
            TranslateCanvas(-2000);
            _prevViewportWidth = ((Grid)mainCanvas.Parent).ActualWidth;
            _prevViewportHeight = ((Grid)mainCanvas.Parent).ActualHeight;

            GraphNodeBase nodeBase = new();
            nodeBase.LoadNodeTypeData("Константа");
            mainCanvas.Children.Add(nodeBase);
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

            for (int px = (int)_gridOffset.X; px < mainCanvas.ActualWidth; px += GRID_STEP)
            {
                for (int py = (int)_gridOffset.Y; py < mainCanvas.ActualHeight; py += GRID_STEP)
                {
                    Ellipse el = new();
                    if ((double)(px / GRID_STEP) % 5 == 0 && (double)(py / GRID_STEP) % 5 == 0) // highlight every 5'th dot
                    {
                        el.Height = DOT_SIZE + 1;
                        el.Width = DOT_SIZE + 1;
                        el.Fill = (Brush)FindResource("Gray_04");
                    }
                    else // regular dot
                    {
                        el.Height = DOT_SIZE;
                        el.Width = DOT_SIZE;
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

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMarkup();
        }

        private void MainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Matrix matrix = matrixTransform.Matrix;
            double scale = e.Delta > 0 ? ZOOM_RATE : 1 / ZOOM_RATE;
            double newZoom = matrix.M11 * scale;

            if (newZoom < MIN_ZOOM || newZoom > MAX_ZOOM)
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

            Point offsetPoint = new(0, 0);

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

        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _canvasDraggingOffset = e.GetPosition(this);
                _draggingCanvas = true;
                mainCanvas.CaptureMouse();
            }
        }

        private void MainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _draggingCanvas = false;
                mainCanvas.ReleaseMouseCapture();
            }
        }

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
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
