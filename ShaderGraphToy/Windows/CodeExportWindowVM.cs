using Microsoft.Win32;
using ShaderGraphToy.Graphics;
using ShaderGraphToy.Utilities.DataBindings;
using System.IO;
using System.Windows;
using System.Windows.Documents;


namespace ShaderGraphToy.Windows
{
    public class CodeExportWindowVM : VmBase
    {
        private string _code = string.Empty;

        public Action<Paragraph>? setCode;

        private int _exportMode = 0;
        public int ExportMode
        {
            get => _exportMode;
            set
            {
                _exportMode = value;
                OnPropertyChanged(nameof(ExportMode));
            }
        }


        public CodeExportWindowVM()
        {
            _code = OpenTkRendererAPI.FragmentCode;
        }

        public void UpdateCode()
        {
            Paragraph p = new();
            p.Inlines.Add(new Run() { Text = _code });

            setCode?.Invoke(p);
        }

        public void OnExportClick(object sender, RoutedEventArgs e)
        {
            if (ExportMode > 0)
            {
                MessageBox.Show("Sorry, but this version of the application only supports exporting GLSL files. Stay tuned for updates!");
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Файлы фрагментного шейдера (*.frag)|*.frag",
                FileName = "fragmentShader",
                OverwritePrompt = true,
                ValidateNames = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, _code);
            }
        }
    }
}
