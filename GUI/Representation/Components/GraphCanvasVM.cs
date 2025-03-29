using GUI.Representation.GraphNodes;
using GUI.Utilities.DataBindings;
using GUI.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GUI.Representation.Components
{
    public class GraphCanvasVM : VmBase
    {
        private RelayCommand? _cursorModeClickCommand = null;
        public RelayCommand CursorModeClickCommand
        {
            get
            {
                _cursorModeClickCommand ??= new RelayCommand(EnableCursorMode);
                return _cursorModeClickCommand;
            }
            set
            {
                _cursorModeClickCommand = value;
                OnPropertyChanged(nameof(CursorModeClickCommand));
            }
        }














        private void EnableCursorMode()
        {

        }

        private void EnableMovementMode()
        {

        }

        private void EnableZoomMode()
        {

        }















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
    }
}
