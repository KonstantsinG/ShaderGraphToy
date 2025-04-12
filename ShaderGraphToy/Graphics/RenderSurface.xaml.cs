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
