using Microsoft.Win32;
using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Utilities.Serializers;
using ShaderGraphToy.Windows;
using System.Windows;


namespace ShaderGraphToy
{
    public class MainWindowVM : VmBase
    {
        private CodeExportWindow? _codeExportWnd;
        private string _projectPath = string.Empty;

        public Func<List<GraphNodeBase>>? getNodes;
        public Action<GraphModel>? openProject;


        public void OpenCodeExportWindow(object sender, RoutedEventArgs e)
        {
            _codeExportWnd?.Close();
            _codeExportWnd = new();
            _codeExportWnd.Show();
        }

        public void SaveProject(object sender, RoutedEventArgs e)
        {
            List<GraphNodeBase>? nodes = getNodes?.Invoke();
            if (nodes == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Файлы проекта ShaderGraphToy (*.gtoy)|*.gtoy",
                FileName = "NewProject",
                OverwritePrompt = true,
                ValidateNames = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _projectPath = saveFileDialog.FileName;
                GraphSerializer.Serialize(nodes, _projectPath);
            }
        }
        public void SaveProjectChanges(object sender, RoutedEventArgs e)
        {
            if (_projectPath == string.Empty)
            {
                SaveProject(sender, e);
                return;
            }

            List<GraphNodeBase>? nodes = getNodes?.Invoke();
            if (nodes == null) return;

            GraphSerializer.Serialize(nodes, _projectPath);
        }
        public void LoadProject(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы проекта ShaderGraphToy (*.gtoy)|*.gtoy",
                ValidateNames = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _projectPath = openFileDialog.FileName;
                GraphModel graph = GraphSerializer.Deserialize(_projectPath);
                openProject?.Invoke(graph);
            }
        }
    }
}
