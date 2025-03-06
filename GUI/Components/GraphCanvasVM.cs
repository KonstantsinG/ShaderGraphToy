using GUI.Controls;
using GUI.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GUI.Components
{
    public class GraphCanvasVM : INotifyPropertyChanged
    {
        private GraphNodesBrowserWindow? _nodesBrowser = null;
        private static int _nodesCounter = 0;

        public Action<GraphNodeBase>? placeNodeOnCanvas;


        public void AddRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _nodesBrowser?.Close();

            _nodesBrowser = new();
            ((GraphNodesBrowserWindowVM)_nodesBrowser.DataContext!).ItemCreated += CreateGraphNode;
            _nodesBrowser.Show();
        }

        public void CreateGraphNode(int? nodeId)
        {
            _nodesBrowser?.Close();
            _nodesBrowser = null;

            if (nodeId != null)
            {
                GraphNodeBase node = new();
                ((GraphNodeBaseVM)node.DataContext!).ConstructNode((int)nodeId);
                node.NodeId = _nodesCounter++;

                placeNodeOnCanvas?.Invoke(node);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
