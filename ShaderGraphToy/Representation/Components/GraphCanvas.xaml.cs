using ShaderGraphToy.Representation.Controls;
using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShaderGraphToy.Representation.Components
{
    /// <summary>
    /// Логика взаимодействия для GraphCanvas.xaml
    /// </summary>
    public partial class GraphCanvas : UserControl
    {
        #region PARAMS
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
        private const int DOT_SIZE = 3;

        // viewport resize params
        private double _prevViewportWidth;
        private double _prevViewportHeight;


        private ConnectorsSpline? _tempSpline;
        private readonly SelectionRect _selectionArea = new();

        private readonly List<GraphNodeBase> _nodes = [];
        private readonly List<GraphNodeBase> _selectedNodes = [];
        private readonly List<ConnectorsSpline> _splines = [];

        private GraphNodeBase? _outputNode = null;

        
        private static readonly Cursor _zoomCursor = ResourceManager.GetCursorFromResources("zoom_cursor.cur");

        private enum MouseInputModes
        {
            Cursor,
            Movement,
            Zoom
        }

        private enum CursorModes
        {
            None,
            NodesMovement,
            SplineDrawing,
            AreaSelecting
        }

        private CursorModes _cursorMode = CursorModes.None;
        private MouseInputModes _mouseInputMode = MouseInputModes.Cursor;
        private MouseInputModes _prevInputMode = MouseInputModes.Cursor;
        private Cursor _prevCursor = Cursors.Arrow;
        #endregion


        public GraphCanvas()
        {
            InitializeComponent();
            TranslateCanvas(-7030);
            _prevViewportWidth = ((Grid)mainCanvas.Parent).ActualWidth;
            _prevViewportHeight = ((Grid)mainCanvas.Parent).ActualHeight;

            cursorLine.Background = (SolidColorBrush)FindResource("Gray_03");

            GraphCanvasVM vm = new() { placeNodeOnCanvas = PlaceNodeOnCanvas };
            DataContext = vm;
        }

        private void TranslateCanvas(double val)
        {
            Matrix matr = matrixTransform.Matrix;
            matr.Translate(val, val);
            matrixTransform.Matrix = matr;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Enable Canvas hotkeys by default
            mainCanvas.Focus();
        }


        #region CANVAS MARKUP
        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawMarkup();
        }

        private void DrawMarkup()
        {
            int segmentSize = GRID_STEP * 5;

            Color gray03 = ((SolidColorBrush)FindResource("Gray_03")).Color;
            Color gray04 = ((SolidColorBrush)FindResource("Gray_04")).Color;

            BitmapSource segment;
            if (ResourceManager.IsCacheExists("bitmaps\\canvasMarkup.png"))
                segment = ResourceManager.GetBitmapFromCache("canvasMarkup.png");
            else
            {
                segment = CreateGridSegment(segmentSize, gray03, gray04);
                ResourceManager.SaveBitmapToCache((WriteableBitmap)segment, "canvasMarkup.png");
            }

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

        private static WriteableBitmap CreateGridSegment(int segmentSize, Color gray03, Color gray04)
        {
            double dpiScale = 1.0;
            double dpi = 96 * dpiScale;

            WriteableBitmap segment = new(
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

        private static void DrawCircle(WriteableBitmap bitmap, int x, int y, Color color, int radius)
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
                        alpha = 1.0;
                        //alpha = 1.0 - (distance - (radius - 0.5)); -> apply anti-aliasing
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
        #endregion



        #region KEYBOARD INPUT
        // Hotkeys only for Canvas focus state
        private void MainCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!mainCanvas.IsKeyboardFocused || e.IsRepeat) return;

            if (e.Key == Key.D1) CursorRect_MouseDown(sender, null);
            else if (e.Key == Key.D2) MoveRect_MouseDown(sender, null);
            else if (e.Key == Key.D3) ZoomRect_MouseDown(sender, null);

            else if (e.Key == Key.F5) ((GraphCanvasVM)DataContext).CompileGraph();
        }

        // global Hotkeys
        public void InvokePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            _shiftPressed = e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift);
            _ctrlPressed = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);

            if (_ctrlPressed && e.Key == Key.N) ((GraphCanvasVM)DataContext)?.AddNodeClickCommand.Execute(null);
            else if (e.Key == Key.Delete) RemoveSelectedNodes();
        }

        public void InvokePreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;

            _shiftPressed = e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift);
            _ctrlPressed = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);
        }
        #endregion

        #region CANVAS MOUSE INPUT
        private void MainCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scale = e.Delta > 0 ? ZOOM_RATE : 1 / ZOOM_RATE;
            Point mousePosition = e.GetPosition(mainCanvas);

            ZoomCanvas(mousePosition, scale);
        }

        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _mouseOffset = e.GetPosition(this);
                _holdingMouse = true;

                _mouseInputMode = MouseInputModes.Movement;
                Cursor = Cursors.SizeAll;
                mainCanvas.CaptureMouse();
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.RightButton == MouseButtonState.Pressed) return;

                _mouseOffset = e.GetPosition(this);
                _holdingMouse = true;

                MainCanvas_PreviewLeftMouseDown(sender, e);
            }
        }

        private void MainCanvas_PreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mouseInputMode == MouseInputModes.Cursor)
            {
                if (e.Source is GraphNodeBase node)
                {
                    GraphNode_PreviewLeftMouseDown(node, e);
                }
                else if (e.Source is Canvas)
                {
                    _cursorMode = CursorModes.AreaSelecting;
                    mainCanvas.CaptureMouse();

                    if (!mainCanvas.Children.Contains(_selectionArea))
                        mainCanvas.Children.Add(_selectionArea);
                    else mainCanvas.Children.Remove(_selectionArea);

                    _selectionArea.Reset();
                    _selectionArea.Offset = e.GetPosition(mainCanvas);
                    _selectionArea.Translate(_selectionArea.Offset);

                    mainCanvas.Focus();
                    if (!_shiftPressed) SelectNode(null);
                }
            }
        }

        private void MainCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _mouseInputMode = _prevInputMode;
                Cursor = _prevCursor;
                _selectionOffset = e.GetPosition(mainCanvas); // reset mouse offset to prevent tp bugs
            }

            if (_cursorMode == CursorModes.SplineDrawing)
                BindSpline(e);
            else if (_cursorMode == CursorModes.AreaSelecting)
                CheckAreaSelection();

            _cursorMode = CursorModes.None;
            mainCanvas.ReleaseMouseCapture();
            _holdingMouse = false;
        }

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_holdingMouse) return;

            switch (_mouseInputMode)
            {
                case MouseInputModes.Cursor:
                    switch (_cursorMode)
                    {
                        case CursorModes.NodesMovement:
                            MoveSelectedNodes(e);
                            break;
                        case CursorModes.AreaSelecting:
                            DrawSelectionArea(e);
                            break;
                        case CursorModes.SplineDrawing:
                            DrawSpline(e);
                            break;
                    }
                    break;

                case MouseInputModes.Movement:
                    MoveCanvas(e);
                    break;

                case MouseInputModes.Zoom:
                    ZoomCanvas(e);
                    break;
            }
        }
        #endregion

        #region GRAPH_NODE MOUSE INPUT
        private void GraphNode_PreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            GraphNodeBase node = (GraphNodeBase)sender;
            _selectionOffset = e.GetPosition(mainCanvas);

            // you can move multiple nodes by grabbing them in any region
            if (_selectedNodes.Count >= 2 && node.Selected)
            {
                _cursorMode = CursorModes.NodesMovement;
                if (!_shiftPressed) return;
            }

            SelectNode(node);
        }

        public void GraphNode_HeaderPanelPressed(object sender, MouseEventArgs e)
        {
            // you can move single node only by grabbing it on header
            _cursorMode = CursorModes.NodesMovement;
        }

        public void GraphNode_ConnectorPressed(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            NodesConnector conn = (NodesConnector)sender;

            // if current connector is input and it already busy - disconnect them from node
            if (conn.IsInput && conn.IsBusy)
            {
                UnbindSpline(conn, e);
                return;
            }    

            conn.IsBusy = true;

            // get global node connector center
            _mouseOffset = conn.GetGlobalCenter(mainCanvas);

            _tempSpline = new();
            Color color1 = ((SolidColorBrush)FindResource("Gray_05")).Color;
            Color color2 = ((SolidColorBrush)FindResource(conn.NodeColor)).Color;

            if (conn.IsInput) _tempSpline.InputConnector = (NodesConnector)sender;
            else _tempSpline.OutputConnector = (NodesConnector)sender;
            
            _tempSpline.Define(_mouseOffset, color1, color2, conn.IsInput);

            mainCanvas.Children.Add(_tempSpline.Path!);
            _cursorMode = CursorModes.SplineDrawing;
        }

        public void GraphNode_NodeStateChanged(GraphNodeBase sender)
        {
            RemoveSplines([sender]);
        }
        #endregion

        #region TOOLBAR HANDLERS
        private void CursorRect_MouseDown(object sender, MouseButtonEventArgs? e)
        {
            _mouseInputMode = MouseInputModes.Cursor;
            _prevInputMode = MouseInputModes.Cursor;
            Cursor = Cursors.Arrow;
            _prevCursor = Cursors.Arrow;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_03");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_005");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_005");
        }

        private void MoveRect_MouseDown(object sender, MouseButtonEventArgs? e)
        {
            _mouseInputMode = MouseInputModes.Movement;
            _prevInputMode = MouseInputModes.Movement;
            Cursor = Cursors.SizeAll;
            _prevCursor = Cursors.SizeAll;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_005");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_03");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_005");
        }

        private void ZoomRect_MouseDown(object sender, MouseButtonEventArgs? e)
        {
            _mouseInputMode = MouseInputModes.Zoom;
            _prevInputMode = MouseInputModes.Zoom;
            Cursor = _zoomCursor;
            _prevCursor = _zoomCursor;
            cursorLine.Background = (SolidColorBrush)FindResource("Gray_005");
            moveLine.Background = (SolidColorBrush)FindResource("Gray_005");
            zoomLine.Background = (SolidColorBrush)FindResource("Gray_03");
        }

        private void RemoveRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveSelectedNodes();
        }
        #endregion



        #region CANVAS TRANSFORMS
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

        // check if this translation will be in canvas region on X
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

        // check if this translation will be in canvas region on Y
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

        private void MoveCanvas(MouseEventArgs e)
        {
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
        }

        private void ZoomCanvas(MouseEventArgs e)
        {
            double delta = e.GetPosition(this).Y - _mouseOffset.Y;

            if (Math.Abs(delta) > 5)
            {
                double scale = delta < 0 ? ZOOM_RATE : 1 / ZOOM_RATE;
                Point mousePosition = e.GetPosition(mainCanvas);
                ZoomCanvas(mousePosition, scale);
                _mouseOffset = e.GetPosition(this);
            }
        }
        #endregion

        #region GRAPH_NODES CONTROLS
        private void AddNode(GraphNodeBase node)
        {
            if (node.NodeTypeId == 3)
            {
                if (_outputNode != null) RemoveNode(_outputNode);
                _outputNode = node;
            }

            _nodes.Add(node);
            mainCanvas.Children.Add(node);
        }

        private void RemoveNode(GraphNodeBase node)
        {
            _nodes.Remove(node);
            mainCanvas.Children.Remove(node);
        }

        public void PlaceNodeOnCanvas(GraphNodeBase node)
        {
            node.HeaderPressed += GraphNode_HeaderPanelPressed;
            node.ConnectorPressed += GraphNode_ConnectorPressed;
            node.NodeStateChanged += GraphNode_NodeStateChanged;
            node.NodeSizeChanged += MoveSplines;
            AddNode(node);

            double px = ((((Grid)mainCanvas.Parent).ActualWidth / matrixTransform.Matrix.M11) / 2) + (-matrixTransform.Matrix.OffsetX / matrixTransform.Matrix.M11);
            double py = (((Grid)mainCanvas.Parent).ActualHeight / matrixTransform.Matrix.M22 / 2) + (-matrixTransform.Matrix.OffsetY / matrixTransform.Matrix.M22);
            node.RenderTransform = new TranslateTransform(px, py);
        }

        private void RemoveSelectedNodes()
        {
            RemoveSplines(_selectedNodes);

            foreach (GraphNodeBase node in _selectedNodes)
                RemoveNode(node);

            ((GraphCanvasVM)DataContext)?.RemoveNodesClickCommand.Execute(null);
            _selectedNodes.Clear();
        }

        public void SelectNode(GraphNodeBase? node)
        {
            if (node != null)
            {
                if (node.Selected && _shiftPressed)
                {
                    node.ToggleSelection(false);
                    _selectedNodes.Remove(node);
                }
                else
                {
                    node.ToggleSelection(true);
                    if (!_selectedNodes.Contains(node)) _selectedNodes.Add(node);
                }
            }

            if (!_shiftPressed)
            {
                foreach (GraphNodeBase n in _nodes)
                {
                    if (node != null && node == n) continue;

                    if (n.Selected)
                    {
                        n.ToggleSelection(false);
                        _selectedNodes.Remove(n);
                    }
                }
            }
        }

        private void MoveSelectedNodes(MouseEventArgs e)
        {
            TranslateTransform tr;
            TranslateTransform oldTr;
            Point currentPosition = e.GetPosition(mainCanvas);

            foreach (GraphNodeBase node in _selectedNodes)
            {
                oldTr = node.RenderTransform as TranslateTransform ?? new(0, 0);
                tr = new(oldTr.X + currentPosition.X - _selectionOffset.X, oldTr.Y + currentPosition.Y - _selectionOffset.Y);

                node.RenderTransform = tr;
                MoveSplines(node);
            }

            _selectionOffset = currentPosition;
        }
        #endregion

        #region SELECTION_AREA CONTROLS
        private void DrawSelectionArea(MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(mainCanvas);

            double x = Math.Min(currentPosition.X, _selectionArea.Offset.X);
            double y = Math.Min(currentPosition.Y, _selectionArea.Offset.Y);

            double width = Math.Abs(currentPosition.X - _selectionArea.Offset.X);
            double height = Math.Abs(currentPosition.Y - _selectionArea.Offset.Y);

            _selectionArea.Update(x, y, width, height);
        }

        private void CheckAreaSelection()
        {
            mainCanvas.Children.Remove(_selectionArea);
            
            foreach (var node in _nodes)
            {
                if (!node.Selected)
                {
                    if (_selectionArea.GetAreaRect().Contains(node.GetBoundsRect()))
                    {
                        node.ToggleSelection(true);
                        _selectedNodes.Add(node);
                    }
                }
            }
        }
        #endregion

        #region SPLINES CONTROLS
        private void MoveSplines(GraphNodeBase node)
        {
            foreach (ConnectorsSpline sp in _splines)
            {
                if (node.ContainsConnector(sp.InputConnector))
                    sp.UpdatePoint(sp.InputConnector!.GetGlobalCenter(mainCanvas), true);
                else if (node.ContainsConnector(sp.OutputConnector))
                    sp.UpdatePoint(sp.OutputConnector!.GetGlobalCenter(mainCanvas), false);
            }
        }

        private void DrawSpline(MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(mainCanvas);
            _tempSpline!.Update(_mouseOffset, currentPoint);
        }

        private void BindSpline(MouseEventArgs e)
        {
            Point endPoint = e.GetPosition(mainCanvas);
            NodesConnector? conn2 = TryFindNodesConnector(endPoint);

            if (IsConnectionPossible(conn2))
            {
                // BIND TWO NODES
                if (_tempSpline!.OutputConnector == null) _tempSpline!.OutputConnector = conn2;
                else _tempSpline!.InputConnector = conn2;

                _tempSpline!.UpdateColor(((SolidColorBrush)FindResource(conn2!.NodeColor)).Color, true);
                conn2.IsBusy = true;
                _tempSpline!.Bezier!.Point3 = conn2.GetGlobalCenter(mainCanvas);

                _tempSpline!.SetConnectionIds();
                _splines.Add(_tempSpline);
            }
            else
            {
                if (_tempSpline!.OutputConnector != null)
                {
                    // output connector may have multiple connections
                    if (_tempSpline.OutputConnector.ConnectionsCount == 0)
                        _tempSpline!.OutputConnector.IsBusy = false;
                }
                else
                {
                    _tempSpline!.InputConnector!.IsBusy = false;
                }

                mainCanvas.Children.Remove(_tempSpline!.Path!);
            }
        }

        private void UnbindSpline(NodesConnector conn, MouseEventArgs e)
        {
            ConnectorsSpline? currSpline = null;

            foreach (ConnectorsSpline sp in _splines)
            {
                if (sp.InputConnector == conn)
                    currSpline = sp;
            }

            if (currSpline != null)
            {
                _splines.Remove(currSpline);
                mainCanvas.Children.Remove(currSpline.Path!);

                currSpline.Break();
                currSpline.OutputConnector!.IsBusy = true;
                currSpline.InputConnector = null;

                _tempSpline = new();
                _tempSpline.Define(currSpline, ((SolidColorBrush)FindResource("Gray_05")).Color);
                mainCanvas.Children.Add(_tempSpline!.Path!);
                _mouseOffset = _tempSpline.GetStartPoint();

                _cursorMode = CursorModes.SplineDrawing;
                DrawSpline(e);
            }
        }

        private void RemoveSplines(List<GraphNodeBase> nodes)
        {
            List<ConnectorsSpline> corpses = [];

            foreach (GraphNodeBase node in nodes)
            {
                foreach (ConnectorsSpline sp in _splines)
                {
                    if (node.ContainsConnector(sp.InputConnector) || node.ContainsConnector(sp.OutputConnector))
                        corpses.Add(sp);
                }

                foreach (ConnectorsSpline corp in corpses)
                {
                    corp.Break();
                    _splines.Remove(corp);
                    mainCanvas.Children.Remove(corp.Path!);
                }
            }
        }

        private bool IsConnectionPossible(NodesConnector? cn)
        {
            if (cn != null)
            {
                // output may have multiple connections, input - only one
                if (cn.IsInput && cn.IsBusy) return false;

                NodesConnector cn2 = _tempSpline!.FreeConnector!;

                // two connectors must be from different nodes and have opposite types
                if (cn != cn2 && cn.NodeId != cn2.NodeId && cn.IsInput != cn2.IsInput)
                    return true;
            }

            return false;
        }

        private NodesConnector? TryFindNodesConnector(Point endPoint)
        {
            NodesConnector? con = null;
            
            VisualTreeHelper.HitTest(mainCanvas, null,
                hitResult =>
                {
                    if (hitResult.VisualHit is Ellipse el)
                    {
                        if (el.Parent is Grid gr && gr.Parent is NodesConnector nc)
                        {
                            con = nc;
                            return HitTestResultBehavior.Stop;
                        }
                    }
                    return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(endPoint)
            );

            return con;
        }
        #endregion
    }
}
