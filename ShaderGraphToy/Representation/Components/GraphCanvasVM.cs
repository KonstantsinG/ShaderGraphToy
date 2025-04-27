using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Windows;
using System.Diagnostics;

namespace ShaderGraphToy.Representation.Components
{
    public class GraphCanvasVM : VmBase
    {
        #region PROPS
        private RelayCommand? _addNodeClickCommand = null;
        public RelayCommand AddNodeClickCommand
        {
            get
            {
                _addNodeClickCommand ??= new RelayCommand(OnAddNodeClicked);
                return _addNodeClickCommand;
            }
            set
            {
                _addNodeClickCommand = value;
                OnPropertyChanged(nameof(AddNodeClickCommand));
            }
        }

        private RelayCommand? _removeNodesClickCommand = null;
        public RelayCommand RemoveNodesClickCommand
        {
            get
            {
                _removeNodesClickCommand ??= new RelayCommand(OnRemoveNodesClicked);
                return _removeNodesClickCommand;
            }
            set
            {
                _removeNodesClickCommand = value;
                OnPropertyChanged(nameof(RemoveNodesClickCommand));
            }
        }
        #endregion

        private GraphNodesBrowserWindow? _nodesBrowser = null;
        private readonly List<GraphNodeBase> _nodes = [];
        private readonly List<(int, int)> _nodesConnections = [];

        public Action<GraphNodeBase>? placeNodeOnCanvas;


        private void OnAddNodeClicked()
        {
            _nodesBrowser?.Close();

            _nodesBrowser = new();
            ((GraphNodesBrowserWindowVM)_nodesBrowser.DataContext!).ItemCreated += CreateGraphNode;
            _nodesBrowser.Show();
        }

        private void OnRemoveNodesClicked()
        {
            List<GraphNodeBase> corpses = [];

            foreach (GraphNodeBase node in _nodes)
            {
                if (node.Selected)
                    corpses.Add(node);
            }

            foreach (GraphNodeBase corp in corpses)
            {
                _nodes.Remove(corp);
                RemoveAllNodeConnections(corp.NodeId);
            }
        }


        public void OnNodesConnectionAdded(int id1, int id2)
        {
            _nodesConnections.Add((id1, id2));
        }

        public void OnNodesConnectionRemoved(int id1, int id2)
        {
            _nodesConnections.Remove((id1, id2));
            _nodesConnections.Remove((id2, id1));
        }

        private void RemoveAllNodeConnections(int id)
        {
            List<(int, int)> corpses = [];

            foreach ((int, int) conn in _nodesConnections)
            {
                if (conn.Item1 == id || conn.Item2 == id)
                    corpses.Add(conn);
            }

            foreach ((int, int) corp in corpses)
                _nodesConnections.Remove(corp);
        }


        public void CreateGraphNode(uint? nodeId)
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

        public void CompileGraph()
        {
            Debug.WriteLine("Shader code compilation started...");


        }
    }
}
