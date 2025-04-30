using Nodes2Shader.Compilation;
using Nodes2Shader.Compilation.MathGraph;
using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Windows;
using System.Diagnostics;
using System.Xml.Linq;

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
        private GraphNodeBase? _outputNode = null;

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
                _nodes.Remove(corp);
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
                if (nodeId == 3)
                {
                    if (_outputNode != null) _nodes.Remove(_outputNode);
                    _outputNode = node;
                }
                placeNodeOnCanvas?.Invoke(node);
            }
        }

        public void CompileGraph()
        {
            if (_nodes.Count < 2) throw new ArgumentException("Graph must contain at least 2 nodes!");
            if (_outputNode == null) throw new ArgumentException("Graph must contain output nodoe!");

            Debug.WriteLine("Shader code compilation started...");

            List<NodeData> nodesData = [];
            RevealGraphLayer([ _outputNode ], nodesData, 0);

            if (nodesData.Count < 2) throw new ArgumentException("Graph must contain output nodoe!");
            GraphData graphData = new() { Nodes = nodesData };

            string code = GraphCompiler.Compile(graphData, out CompilationResult result);
        }

        private void RevealGraphLayer(List<GraphNodeBase> startNodes, List<NodeData> revealed, int layer)
        {
            NodeData data;
            List<GraphNodeBase> nextGen = [];

            if (startNodes.Count == 0 || layer > 999) return;

            foreach (GraphNodeBase node in startNodes)
            {
                data = node.GetNodeData();
                data.Layer = layer;
                revealed.Add(data);

                nextGen.AddRange(FindNodes(data.InputConnections, data.Id));
            }

            RevealGraphLayer(nextGen, revealed, ++layer);
        }

        private List<GraphNodeBase> FindNodes(List<NodesConnection> conns, int ownId)
        {
            List<GraphNodeBase> nodes = [];

            foreach (GraphNodeBase gn in _nodes)
            {
                if (gn.NodeId == ownId) continue;

                if (conns.Any(c => c.FirstNodeId == gn.NodeId || c.SecondNodeId == gn.NodeId))
                    nodes.Add(gn);
            }

            return nodes;
        }
    }
}
