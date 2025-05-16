using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Windows;
using System.Windows;
using System.Windows.Input;


namespace ShaderGraphToy
{
    public class MainWindowVM : VmBase
    {
        private CodeExportWindow? _codeExportWnd;


        public void OpenCodeExportWindow(object sender, RoutedEventArgs e)
        {
            _codeExportWnd?.Close();
            _codeExportWnd = new();
            _codeExportWnd.Show();
        }
    }
}
