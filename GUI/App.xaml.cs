using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private nint _nvapiIdx = nint.Zero;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EnableGPUHighPerformance();
            Exit += AppExit;
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            if (_nvapiIdx != nint.Zero)
                NativeLibrary.Free(_nvapiIdx);
        }



        private void EnableGPUHighPerformance()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    _nvapiIdx = NativeLibrary.Load("nvapi64.dll");
                else
                    _nvapiIdx = NativeLibrary.Load("nvapi32.dll");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GPU high performance setup failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
