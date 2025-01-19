using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using System.Windows.Controls;

namespace GUI.Components
{
    /// <summary>
    /// Логика взаимодействия для RenderingViewport.xaml
    /// </summary>
    public partial class RenderingViewport : UserControl
    {
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
    }
}
