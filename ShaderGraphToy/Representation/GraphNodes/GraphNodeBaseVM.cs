using ShaderGraphToy.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ShaderGraphToy.Representation.GraphNodes.GraphNodeComponents;
using System.Windows.Input;
using Nodes2Shader.GraphNodesImplementation.Contents;
using Nodes2Shader.GraphNodesImplementation.Types;
using Nodes2Shader.Serializers;
using ShaderGraphToy.Utilities.DataBindings;
using Nodes2Shader.Compilation.MathGraph;

namespace ShaderGraphToy.Representation.GraphNodes
{
    public class GraphNodeBaseVM : VmBase
    {
        #region PROPS
        private static int _nodesCounter = 0;
        private int _inputsCounter = 0;
        private int _outputsCounter = 0;

        public int NodeId { get; private set; }
        public List<NodesConnector> Connectors { get; private set; } = [];

        public List<NodesConnector> Inputs
        {
            get => Connectors.Where(c => c.IsInput).ToList();
        }
        public List<NodesConnector> Outputs
        {
            get => Connectors.Where(c => !c.IsInput).ToList();
        }

        private GraphNodeContent? _contentModel;
        public GraphNodeContent? ContentModel
        {
            get => _contentModel;
            set
            {
                _contentModel = value;
                OnPropertyChanged(nameof(ContentModel));
            }
        }

        private GraphNodeType? _nodeModel;
        public GraphNodeType? NodeModel
        {
            get => _nodeModel;
            set
            {
                _nodeModel = value;
                OnPropertyChanged(nameof(NodeModel));
            }
        }

        private bool _isConnectorsVisible = false;
        public bool IsConnectorsVisible
        {
            get => _isConnectorsVisible;
            set
            {
                _isConnectorsVisible = value;
                OnPropertyChanged(nameof(IsConnectorsVisible));
            }
        }

        private int _selectedOperationIndex = -1;
        public int SelectedOperationIndex
        {
            get => _selectedOperationIndex;
            set
            {
                _selectedOperationIndex = value;
                OnPropertyChanged(nameof(SelectedOperationIndex));
            }
        }

        private int _selectedSubOperationIndex = -1;
        public int SelectedSubOperationIndex
        {
            get => _selectedSubOperationIndex;
            set
            {
                _selectedSubOperationIndex = value;
                OnPropertyChanged(nameof(SelectedSubOperationIndex));
            }
        }

        public ObservableCollection<OperationType> NodeOperations { get; set; } = [];
        public ObservableCollection<OperationSubType> NodeSubOperations { get; set; } = [];
        public ObservableCollection<INodeComponentView> NodeComponents { get; set; } = [];

        public Func<bool, bool, List<NodesConnector>>? GetOwnConnectors;
        public Action<object, MouseEventArgs>? RaiseConnectorPressedEvent;
        public Action? RaiseNodeSizeChangedEvent;
        public Action? HideOperationsCBoxes;
        #endregion


        public GraphNodeBaseVM()
        {
            NodeId = _nodesCounter++;
        }

        public List<NodeEntry> GetComponentsEntries()
        {
            List<NodeEntry> entries = [];
            NodeEntry? entry;

            foreach (INodeComponentView compView in NodeComponents)
            {
                entry = compView.GetData();
                if (entry != null) entries.Add(entry);
            }

            return entries;
        }

        public List<NodesConnection> GetInputConnections()
        {
            List<NodesConnection> cons = [];

            foreach (NodesConnector nc in Connectors)
            {
                if (!nc.IsBusy || !nc.IsInput) continue;
                for (int i = 0; i < nc.ConnectedNodesIds.Count; i++)
                    cons.Add(new(nc.NodeId, nc.ConnectedNodesIds[i], nc.ConnectorId, nc.ConnectedConnectorsIds[i]));

            }

            return cons;
        }

        public List<NodesConnection> GetOutputConnections()
        {
            List<NodesConnection> cons = [];

            foreach (NodesConnector nc in Connectors)
            {
                if (!nc.IsBusy || nc.IsInput) continue;
                for (int i = 0; i < nc.ConnectedNodesIds.Count; i++)
                    cons.Add(new(nc.NodeId, nc.ConnectedNodesIds[i], nc.ConnectorId, nc.ConnectedConnectorsIds[i]));
            }

            return cons;
        }

        public NodeEntry? GetNodeInput()
        {
            if (!ContentModel!.HasInput) return null;

            NodeEntry input = new()
            {
                Type = ContentModel!.InputType,
                Behavior = NodeEntry.EntryType.Input,
                Id = 0
            };

            return input;
        }

        public NodeEntry? GetNodeOutput()
        {
            if (!ContentModel!.HasOutput) return null;

            NodeEntry output = new()
            {
                Type = ContentModel!.OutputType,
                Behavior = NodeEntry.EntryType.Output,
                Id = 0
            };

            return output;
        }

        public NodeData GetNodeData()
        {
            NodeData data = new(NodeId)
            {
                TypeId = (int)ContentModel!.Id,
                NodeInput = GetNodeInput(),
                NodeOutput = GetNodeOutput(),
                Entries = GetComponentsEntries(),
                InputConnections = GetInputConnections(),
                OutputConnections = GetOutputConnections()
            };

            return data;
        }

        public void OperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!NodeModel!.UsingSubOperations)
            {
                uint id = (((ComboBox)sender).SelectedItem as OperationType)!.Id;
                LoadNodeContent(id);
            }
            else
            {
                NodeSubOperations.Clear();
                var subs = (((ComboBox)sender).SelectedItem as OperationType)!.OperationsSubTypes;
                foreach (var sub in subs) NodeSubOperations.Add(sub);
            }
        }

        public void SubOperationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem is OperationSubType item)
            {
                LoadNodeContent(item.Id);
            }
            else
            {
                ContentModel = null;
                NodeComponents.Clear();
                IsConnectorsVisible = false;
            }
        }

        private void DefineConnectors()
        {
            _inputsCounter = 0; _outputsCounter = 0;
            Connectors.Clear();

            Connectors.AddRange(GetOwnConnectors!.Invoke(ContentModel!.HasInput, ContentModel.HasOutput));

            foreach (INodeComponentView comp in NodeComponents)
            {
                if (comp is InputComponentView inputComp)
                {
                    NodesConnector? conn = inputComp.GetConnector();
                    if (conn != null) Connectors.Add(conn);
                }
                else if (comp is InscriptionComponentView inscComp)
                    Connectors.AddRange(inscComp.GetConnectors());
            }

            foreach (NodesConnector con in Connectors)
            {
                if (con.IsInput) con.ConnectorId = _inputsCounter++;
                else con.ConnectorId = _outputsCounter++;

                con.NodeId = NodeId;
                con.NodeColor = NodeModel!.Color;
                con.MouseDown += NodesConnector_MouseDown;
            }
        }

        public void LoadNodeContent(uint id)
        {
            var compsData = GraphNodesContentsSerializer.Deserialize(id);
            ContentModel = compsData!;

            var comps = GraphComponentsFactory.ConstructComponents(compsData!.Components);
            NodeComponents.Clear();
            foreach (var comp in comps)
            {
                if (comp is ColorComponentView colorComp)
                    colorComp.ComponentSizeChanged += OnComponentSizeChaged;

                NodeComponents.Add(comp);
            }

            // setup all connectors data and id's
            DefineConnectors();
            IsConnectorsVisible = true;
        }

        public void ConstructNode(int nodeId)
        {
            string strId = nodeId.ToString();
            uint idFirstPart = uint.Parse(strId[0].ToString());

            GraphNodeType info = GraphNodesTypesSerializer.Deserialize(idFirstPart)!;
            NodeModel = info;

            if (!info.UsingOperations)
            {
                uint id = info.Id;
                LoadNodeContent(id);
                HideOperationsCBoxes?.Invoke();
            }
            else
            {
                NodeOperations.Clear();
                foreach (var op in info.OperationsTypes) NodeOperations.Add(op);

                if (strId.Length == 2)
                {
                    SelectedOperationIndex = int.Parse(strId[1].ToString()) - 1;
                    if (!info.UsingSubOperations) HideOperationsCBoxes?.Invoke();
                }
                else if (strId.Length == 3)
                {
                    SelectedOperationIndex = int.Parse(strId[1].ToString()) - 1;
                    SelectedSubOperationIndex = int.Parse(strId[2].ToString()) - 1;
                    HideOperationsCBoxes?.Invoke();
                }
            }
        }

        public void NodesConnector_MouseDown(object sender, MouseEventArgs e)
        {
            RaiseConnectorPressedEvent?.Invoke(sender, e);
        }

        private void OnComponentSizeChaged()
        {
            RaiseNodeSizeChangedEvent?.Invoke();
        }
    }
}
