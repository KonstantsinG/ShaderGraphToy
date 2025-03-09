using GUI.Controls;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для GraphCanvas.xaml
    /// </summary>
    public partial class GraphCanvas : UserControl
    {
        private Point _mouseOffset;
        private Point _selectionOffset;
        private bool _holdingMouse = false;

        private bool _shiftPressed = false;
        private bool _ctrlPressed = false;

        // Canvas zoom params
        private const double ZOOM_RATE = 1.1;
        private const double MIN_ZOOM = 0.4;
        private const double MAX_ZOOM = 2.0;

        // Canvas markup params
        private const int GRID_STEP = 60;
        private Point _gridOffset = new(30, 30);
        private const int DOT_SIZE = 3;
        private WriteableBitmap? _gridBitmap;

        // viewport resize params
        private double _prevViewportWidth;
        private double _prevViewportHeight;

        public delegate void NodeSelectionHandler(GraphNodeBase? node, bool shiftPressed = false);
        public delegate void NodesActionsHandler(List<GraphNodeBase> nodes);

        public event EventHandler NodesBrowserOpened = delegate { };
        public event NodeSelectionHandler NodeSelectionToggled = delegate { };
        public event NodesActionsHandler NodesRemoved = delegate { };
        
        public static Cursor ZoomCursor { get; } = new Cursor(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/zoom_cursor.cur"));
        
        private List<GraphNodeBase> _selectedNodes = [];

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

            GraphCanvasVM vm = new() { placeNodeOnCanvas = PlaceNodeOnCanvas };
            NodesBrowserOpened += vm.OpenNodesBrowser;
            NodeSelectionToggled += vm.SelectNode;
            NodesRemoved += vm.RemoveNodes;
            DataContext = vm;
        }

        private void TranslateCanvas(double val)
        {
            Matrix matr = matrixTransform.Matrix;
            matr.Translate(val, val);
            matrixTransform.Matrix = matr;
        }


        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMarkup();
        }

        private void DrawMarkup()
        {
            int segmentSize = GRID_STEP * 5;

            Color gray03 = ((SolidColorBrush)FindResource("Gray_03")).Color;
            Color gray04 = ((SolidColorBrush)FindResource("Gray_04")).Color;

            WriteableBitmap segment = CreateGridSegment(segmentSize, gray03, gray04);

            ImageBrush gridBrush = new()
            {
                ImageSource = segment,
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, segmentSize, segmentSize),
                ViewportUnits = BrushMappingMode.Absolute
            };

            Rectangle gridRect = new()
            {
                Width = mainCanvas.ActualWidth,
                Height = mainCanvas.ActualHeight,
                Fill = gridBrush,
                IsHitTestVisible = false
            };

            Panel.SetZIndex(gridRect, -4);
            mainCanvas.Children.Add(gridRect);
        }

        private WriteableBitmap CreateGridSegment(int segmentSize, Color gray03, Color gray04)
        {
            double dpiScale = 1.0;
            double dpi = 96 * dpiScale;

            WriteableBitmap segment = new WriteableBitmap(
                (int)(segmentSize * dpiScale),
                (int)(segmentSize * dpiScale),
                dpi, dpi, PixelFormats.Pbgra32, null);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    bool isBigDot = i == 0 && j == 0;
                    Color color = isBigDot ? gray04 : gray03;

                    int radius = isBigDot ? DOT_SIZE / 2 + 1 : DOT_SIZE / 2;
                    DrawCircle(segment, i * GRID_STEP + GRID_STEP / 2, j * GRID_STEP + GRID_STEP / 2, color, radius);
                }
            }

            return segment;
        }

        private void DrawCircle(WriteableBitmap bitmap, int x, int y, Color color, int radius)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    double distance = Math.Sqrt(i * i + j * j);
                    double alpha = 1.0;

                    if (distance > radius - 0.5 && distance <= radius + 0.5)
                        alpha = 1.0 - (distance - (radius - 0.5));
                    else if (distance > radius + 0.5)
                        continue;

                    int px = x + i;
                    int py = y + j;

                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        byte[] pixelData = [color.B, color.G, color.R, (byte)(color.A * alpha)];
                        bitmap.WritePixels(new Int32Rect(px, py, 1, 1), pixelData, bytesPerPixel, 0);
                    }
                }
            }
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

        public void InvokePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            _shiftPressed = e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift);
            _ctrlPressed = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            if (_ctrlPressed && e.Key == Key.N) NodesBrowserOpened.Invoke(sender, e);
            else if (e.Key == Key.Delete) RemoveSelectedNodes();
        }

        public void InvokePreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            _shiftPressed = e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift);
            _ctrlPressed = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);
        }

        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseOffset = e.GetPosition(this);
            _holdingMouse = true;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_mouseInputMode != MouseInputModes.Cursor) // you need to leave an opportunity for cursor to interact with the canvas children
                    mainCanvas.CaptureMouse();
                else
                {
                    if (e.Source is GraphNodeBase node) // if you press on the GraphNode, you need to select them and deselect others
                    {
                        _selectionOffset = e.GetPosition(mainCanvas);

                        if (_shiftPressed)
                        {
                            if (node.Selected) _selectedNodes.Remove(node); // if shift pressed and this node is already selected - deselect him
                            else if (!_selectedNodes.Contains(node)) _selectedNodes.Add(node);
                        }
                        else
                        {
                            if (_selectedNodes.Count > 1) return; // if some nodes selected and shift is up - do nothing with selection area

                            _selectedNodes.Clear();
                            _selectedNodes.Add(node);
                        }

                        NodeSelectionToggled.Invoke(node, _shiftPressed);
                    }
                    else if (e.Source is Canvas) // if you press ont the canvas, you need to deselect all the GraphNodes
                    {
                        _selectedNodes.Clear();
                        NodeSelectionToggled.Invoke(null);
                    }
                }
            }
            else if (e.MiddleButton == MouseButtonState.Pressed) // mouse wheel always for movement state
            {
                _mouseInputMode = MouseInputModes.Movement;
                Cursor = Cursors.SizeAll;
                mainCanvas.CaptureMouse();
            }
        }

        private void MainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _mouseInputMode = _prevInputMode;
                Cursor = _prevCursor;
            }

            mainCanvas.ReleaseMouseCapture();
            _holdingMouse = false;
        }

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_holdingMouse) return;

            switch (_mouseInputMode)
            {
                case MouseInputModes.Cursor:
                    TryMoveSelectedNodes(e);

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

        private void AddRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NodesBrowserOpened.Invoke(sender, e);
        }

        private void RemoveRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveSelectedNodes();
        }


        public void PlaceNodeOnCanvas(GraphNodeBase node)
        {
            mainCanvas.Children.Add(node);

            double px = ((((Grid)mainCanvas.Parent).ActualWidth / matrixTransform.Matrix.M11) / 2) + (-matrixTransform.Matrix.OffsetX / matrixTransform.Matrix.M11);
            double py = (((Grid)mainCanvas.Parent).ActualHeight / matrixTransform.Matrix.M22 / 2) + (-matrixTransform.Matrix.OffsetY / matrixTransform.Matrix.M22);
            node.RenderTransform = new TranslateTransform(px, py);
        }

        private void RemoveSelectedNodes()
        {
            foreach (GraphNodeBase node in _selectedNodes)
                mainCanvas.Children.Remove(node);

            NodesRemoved.Invoke(_selectedNodes);
            _selectedNodes.Clear();
        }

        private bool TryMoveSelectedNodes(MouseEventArgs e)
        {
            if (_selectedNodes.Count == 0) return false;

            TranslateTransform tr;
            TranslateTransform oldTr;
            Point currentPosition = e.GetPosition(mainCanvas);

            foreach (GraphNodeBase node in _selectedNodes)
            {
                oldTr = node.RenderTransform as TranslateTransform ?? new(0, 0);
                tr = new(oldTr.X + currentPosition.X - _selectionOffset.X, oldTr.Y + currentPosition.Y - _selectionOffset.Y);

                node.RenderTransform = tr;
            }

            _selectionOffset = currentPosition;

            return true;
        }
    }
}
