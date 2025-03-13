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

        private readonly List<GraphNodeBase> _nodes = [];

        public Action<GraphNodeBase>? placeNodeOnCanvas;


        public void OpenNodesBrowser(object? sender, EventArgs? e)
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

                _nodes.Add(node);
                placeNodeOnCanvas?.Invoke(node);
            }
        }

        public void SelectNode(GraphNodeBase? node, bool shiftPressed)
        {
            if (node != null)
            {
                if (node.Selected && shiftPressed)
                    node.ToggleSelection(false);
                else
                    node.ToggleSelection(true);
            }

            if (!shiftPressed)
            {
                foreach (GraphNodeBase n in _nodes)
                {
                    if (node != null && node == n) continue;

                    if (n.Selected) n.ToggleSelection(false);
                }
            }
        }

        public void RemoveNodes(List<GraphNodeBase> nodes)
        {
            foreach (GraphNodeBase node in nodes)
                _nodes.Remove(node);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
