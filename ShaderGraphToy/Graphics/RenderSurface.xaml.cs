using ShaderGraphToy.DllWrappers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShaderGraphToy.Graphics
{
    /// <summary>
    /// Логика взаимодействия для RenderSurface.xaml
    /// </summary>
    public partial class RenderSurface : UserControl, IDisposable
    {
        private RenderSurfaceHost? _host = null;

        public RenderSurface()
        {
            InitializeComponent();
            Loaded += OnRenderSurfaceLoaded;
        }

        private void OnRenderSurfaceLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnRenderSurfaceLoaded;

            _host = new RenderSurfaceHost(ActualWidth, ActualHeight);
            Content = _host;
        }


        #region IDISPOSABLE
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _host?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
