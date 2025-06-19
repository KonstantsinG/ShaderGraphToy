using Nodes2Shader.Compilation;
using Nodes2Shader.Compilation.MathGraph;
using ShaderGraphToy.Graphics;
using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Utilities.Serializers;
using ShaderGraphToy.Windows;
using System.Diagnostics;
using System.Windows.Media;

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
        public Action<string>? raiseError;


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
                if (corp == _outputNode) _outputNode = null;
                _nodes.Remove(corp);
            }
        }


        public void CreateGraphNode(uint? nodeId)
        {
            _nodesBrowser?.Close();
            _nodesBrowser = null;

            if (nodeId != null)
            {
                GraphNodeBase node = new();
                node.Construct((int)nodeId);

                _nodes.Add(node);
                if (nodeId == 3)
                {
                    if (_outputNode != null) _nodes.Remove(_outputNode);
                    _outputNode = node;
                }
                placeNodeOnCanvas?.Invoke(node);
            }
        }

        public void CreateGraphNode(NodeModel model)
        {
            GraphNodeBase node = new(model.Id);
            node.Construct(model.TypeId);

            _nodes.Add(node);
            if (model.TypeId == 3) _outputNode = node;
            placeNodeOnCanvas?.Invoke(node);
            // update node layout to load content (events will only work after update)
            node.UpdateLayout();

            node.RenderTransform = new TranslateTransform(model.TranslateX, model.TranslateY);
            node.SetContents(model.Contents);
            node.UpdateLayout(); // wrong output node connectors positions fix
        }

        public void CompileGraph()
        {
            try
            {
                if (_nodes.Count < 2) throw new ArgumentException("Graph must contain at least 2 nodes!");
                if (_outputNode == null) throw new ArgumentException("Graph must contain output node!");

                List<NodeData> nodesData = [];
                RevealGraphLayer([_outputNode], nodesData, 0);
                // nodes that have no outputs have been excluded from the graph, so you need to remove all connections to these nodes
                RemoveExcludedOutputs(nodesData);

                if (nodesData.Count < 2) throw new ArgumentException("Graph must contain at least 2 nodes!");
                foreach (NodeData node in nodesData) CheckForNotImplemented(node); // exceptions for all not implemented features
                GraphData graphData = new() { Nodes = nodesData };

                string code = GraphCompiler.Compile(graphData, out string[] uniforms);
                OpenTkRendererAPI.RenderFragmentShader(code, uniforms);
            }
            catch (Exception ex)
            {
                raiseError?.Invoke(ex.Message);
            }
        }

        private static void CheckForNotImplemented(NodeData node)
        {
            int[] mats = [ 113, 124, 125, 425, 431, 432, 433, 434, 435, 436 ];

            if (mats.Any(m => m == node.TypeId)) throw new NotImplementedException("Sorry, but this version of the app does not support matrix operations. Stay tuned for updates!");
            if (node.TypeId == 526) throw new NotImplementedException("Sorry, but this version of the app does not have remap function implementation. Stay tuned for updates!");
            if (node.TypeId == 532 || node.TypeId == 534) throw new NotImplementedException("Sorry, but this version of the app does not have arc-functions implementations. Stay tuned for updates!");
            if (node.TypeId == 23) throw new NotImplementedException("Sorry, but this version of the app does not support mouse position. Stay tuned for updates!");
        }

        private void RevealGraphLayer(List<GraphNodeBase> startNodes, List<NodeData> revealed, int layer)
        {
            NodeData data;
            List<GraphNodeBase> nextGen = [];
            NodeData? same;

            if (startNodes.Count == 0 || layer > 999) return;

            foreach (GraphNodeBase node in startNodes)
            {
                data = node.GetNodeData();
                data.Layer = layer;

                same = revealed.Where(r => r.Id == data.Id).FirstOrDefault();
                // if the same node is not found - add it
                if (same == null) revealed.Add(data);
                // otherwise - update the layer for the existing node (this means that the node is connected to other nodes from different layers,
                //                                                     so its value should be revealed as soon as possible)
                else same.Layer = data.Layer;


                foreach(GraphNodeBase n in FindNodes(data.InputConnections, data.Id))
                    if (!nextGen.Contains(n)) nextGen.Add(n);
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

        private static void RemoveExcludedOutputs(List<NodeData> nodes)
        {
            List<int> ids = nodes.Select(n => n.Id).ToList();
            List<NodesConnection> corpses = [];

            foreach (NodeData nd in nodes)
            {
                foreach (NodesConnection con in nd.OutputConnections)
                {
                    if (!ids.Contains(con.FirstNodeId) || !ids.Contains(con.SecondNodeId))
                        corpses.Add(con);
                }

                if (corpses.Count > 0)
                {
                    foreach (NodesConnection corp in corpses)
                        nd.OutputConnections.Remove(corp);
                    corpses.Clear();
                }
            }
        }
    }
}
