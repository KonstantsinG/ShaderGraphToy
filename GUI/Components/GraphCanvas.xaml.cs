using GUI.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для GraphCanvas.xaml
    /// </summary>
    public partial class GraphCanvas : UserControl
    {
        private Point _mouseOffset;
        private bool _holdingMouse = false;

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
        
        public static Cursor ZoomCursor { get; } = new Cursor(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/zoom_cursor.cur"));

        private enum MouseInputModes
        {
            Cursor,
            Movement,
            Zoom
        }

        private MouseInputModes _mouseInputMode = MouseInputModes.Cursor;
        private MouseInputModes _prevInputMode = MouseInputModes.Cursor;
        private Cursor _prevCursor = Cursors.Arrow;


        public GraphCanvas()
        {
            InitializeComponent();
            TranslateCanvas(-2000);
            _prevViewportWidth = ((Grid)mainCanvas.Parent).ActualWidth;
            _prevViewportHeight = ((Grid)mainCanvas.Parent).ActualHeight;

            cursorLine.Background = (SolidColorBrush)FindResource("Gray_03");

            GraphCanvasVM vm = new()
            {
                placeNodeOnCanvas = PlaceNodeOnCanvas
            };
            addRect.MouseDown += vm.AddRect_MouseDown;
            DataContext = vm;
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
            double scale = e.Delta > 0 ? ZOOM_RATE : 1 / ZOOM_RATE;
            Point mousePosition = e.GetPosition(mainCanvas);

            ZoomCanvas(mousePosition, scale);
        }

        private void ZoomCanvas(Point mousePosition, double scale)
        {
            Matrix matrix = matrixTransform.Matrix;
            double newZoom = matrix.M11 * scale;

            if (newZoom < MIN_ZOOM || newZoom > MAX_ZOOM)
                return;

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
                _mouseOffset = e.GetPosition(this);
                _mouseInputMode = MouseInputModes.Movement;
                Cursor = Cursors.SizeAll;
                mainCanvas.CaptureMouse();
                _holdingMouse = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseOffset = e.GetPosition(this);
                _holdingMouse = true;

                if (_mouseInputMode != MouseInputModes.Cursor)
                    mainCanvas.CaptureMouse();
            }
        }

        private void MainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _mouseInputMode = _prevInputMode;
                Cursor = _prevCursor;
                mainCanvas.ReleaseMouseCapture();
                _holdingMouse = false;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                mainCanvas.ReleaseMouseCapture();
                _holdingMouse = false;
            }
        }

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_holdingMouse) return;

            switch (_mouseInputMode)
            {
                case MouseInputModes.Cursor:
                    // selection area stuff
                    break;

                case MouseInputModes.Movement:
                    Point mousePos = e.GetPosition(this);
                    Matrix matrix = matrixTransform.Matrix;

                    double translateValueX = mousePos.X - _mouseOffset.X;
                    double translateValueY = mousePos.Y - _mouseOffset.Y;
                    bool translateX = CheckTranslationX(translateValueX);
                    bool translateY = CheckTranslationY(translateValueY);

                    if (translateX) matrix.Translate(translateValueX, 0);
                    if (translateY) matrix.Translate(0, translateValueY);

                    _mouseOffset = mousePos;
                    matrixTransform.Matrix = matrix;
                    break;

                case MouseInputModes.Zoom:
                    double delta = e.GetPosition(this).Y - _mouseOffset.Y;
                    if (Math.Abs(delta) > 5)
                    {
                        double scale = delta < 0 ? ZOOM_RATE : 1 / ZOOM_RATE;
                        Point mousePosition = e.GetPosition(mainCanvas);
                        ZoomCanvas(mousePosition, scale);
                        _mouseOffset = e.GetPosition(this);
                    }
                    break;
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



        private void CursorRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseInputMode = MouseInputModes.Cursor;
            _prevInputMode = MouseInputModes.Cursor;
            Cursor = Cursors.Arrow;
            _prevCursor = Cursors.Arrow;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_03");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_005");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_005");
        }

        private void MoveRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseInputMode = MouseInputModes.Movement;
            _prevInputMode = MouseInputModes.Movement;
            Cursor = Cursors.SizeAll;
            _prevCursor = Cursors.SizeAll;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_005");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_03");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_005");
        }

        private void ZoomRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseInputMode = MouseInputModes.Zoom;
            _prevInputMode = MouseInputModes.Zoom;
            Cursor = ZoomCursor;
            _prevCursor = ZoomCursor;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_005");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_005");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_03");
        }


        public void PlaceNodeOnCanvas(GraphNodeBase node)
        {
            mainCanvas.Children.Add(node);

            double px = ((((Grid)mainCanvas.Parent).ActualWidth / matrixTransform.Matrix.M11) / 2) + (-matrixTransform.Matrix.OffsetX / matrixTransform.Matrix.M11);
            double py = (((Grid)mainCanvas.Parent).ActualHeight / matrixTransform.Matrix.M22 / 2) + (-matrixTransform.Matrix.OffsetY / matrixTransform.Matrix.M22);
            Canvas.SetLeft(node, px);
            Canvas.SetTop(node, py);
        }
    }
}
