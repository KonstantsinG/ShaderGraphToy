using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для RenderingViewport.xaml
    /// </summary>
    public partial class RenderingViewport : UserControl
    {
        private bool _isDraggingResizeRect = false;
        private Point _resizeStartPoint;


        public RenderingViewport()
        {
            InitializeComponent();
            DataContext = new RenderingViewportVM();

            BindRenderingViewport();
        }


        private void BindRenderingViewport()
        {
            openTkControl.SizeChanged += (s, e) =>
            {
                ((RenderingViewportVM)DataContext).ViewportWidth = openTkControl.ActualWidth;
                ((RenderingViewportVM)DataContext).ViewportHeight = openTkControl.ActualHeight;
            };

            openTkControl.Ready += ((RenderingViewportVM)DataContext).OpenTkControl_Ready;
            openTkControl.Render += ((RenderingViewportVM)DataContext).OpenTkControl_OnRender;

            var settings = new GLWpfControlSettings
            {
                MajorVersion = 3,
                MinorVersion = 3
            };
            openTkControl.Start(settings);
        }



        private void ResizeRectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingResizeRect = true;
            _resizeStartPoint = e.GetPosition(this);
            resizeRect.CaptureMouse();
        }

        private void ResizeRectangle_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingResizeRect = false;
            resizeRect.ReleaseMouseCapture();
        }

        private void ResizeRectangle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDraggingResizeRect)
            {
                Point currentPoint = e.GetPosition(this);
                double deltaY = -(currentPoint.Y - _resizeStartPoint.Y);

                double totalHeight = infoRow.ActualHeight + viewportRow.ActualHeight;
                double newInfoHeight = infoRow.ActualHeight + deltaY;
                double newViewportHeight = totalHeight - newInfoHeight;

                double minHeight = totalHeight * 0.2;
                if (newInfoHeight < minHeight || newViewportHeight < minHeight)
                    return;

                double infoRatio = newInfoHeight / totalHeight;
                double viewportRatio = newViewportHeight / totalHeight;

                infoRow.Height = new GridLength(infoRatio, GridUnitType.Star);
                viewportRow.Height = new GridLength(viewportRatio, GridUnitType.Star);

                _resizeStartPoint = currentPoint;
            }
        }
    }
}
