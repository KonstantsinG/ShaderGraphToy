using Nodes2Shader.Serializers;
using System.Windows;

namespace ShaderGraphToy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDraggingResizeRect = false;
        private Point _resizeStartPoint;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void ResizeRect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingResizeRect = true;
            _resizeStartPoint = e.GetPosition(this);
            resizeRect.CaptureMouse();
        }

        private void ResizeRect_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingResizeRect = false;
            resizeRect.ReleaseMouseCapture();
        }

        private void ResizeRect_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDraggingResizeRect)
            {
                Point currentPoint = e.GetPosition(this);
                double deltaX = currentPoint.X - _resizeStartPoint.X;

                double totalWidth = canvasColumn.ActualWidth + viewportColumn.ActualWidth;
                double newCanvasWidth = canvasColumn.ActualWidth + deltaX;
                double newViewportWidth = totalWidth - newCanvasWidth;

                double minWidth = totalWidth * 0.2;
                if (newCanvasWidth < minWidth || newViewportWidth < minWidth)
                    return;

                double canvasRatio = newCanvasWidth / totalWidth;
                double viewportRatio = newViewportWidth / totalWidth;

                canvasColumn.Width = new GridLength(canvasRatio, GridUnitType.Star);
                viewportColumn.Width = new GridLength(viewportRatio, GridUnitType.Star);

                _resizeStartPoint = currentPoint;
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            graphCanvas.InvokePreviewKeyDown(sender, e);
        }

        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            graphCanvas.InvokePreviewKeyUp(sender, e);
        }
    }
}