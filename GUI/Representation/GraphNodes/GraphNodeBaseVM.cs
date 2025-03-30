using GUI.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using GUI.Representation.GraphNodes.GraphNodeComponents;
using System.Windows.Input;
using ShaderGraph.GraphNodesImplementation.Contents;
using ShaderGraph.GraphNodesImplementation.Types;
using ShaderGraph.Serializers;
using GUI.Utilities.DataBindings;

namespace GUI.Representation.GraphNodes
{
    public class GraphNodeBaseVM : VmBase
    {
        #region PROPS
        private static int _nodesCounter = 0;
        private static int _inputsCounter = 0;
        private static int _outputsCounter = 0;

        public int NodeId { get; private set; }
        public List<NodesConnector> Connectors { get; private set; } = [];

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
        public ObservableCollection<UserControl> NodeComponents { get; set; } = [];

        public Func<List<NodesConnector>>? GetOwnConnectors;
        public Action<object, MouseEventArgs>? RaiseConnectorPressedEvent;
        public Action? HideOperationsCBoxes;
        #endregion


        public GraphNodeBaseVM()
        {
            NodeId = _nodesCounter++;
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
            Connectors.AddRange(GetOwnConnectors!.Invoke());

            foreach (UserControl comp in NodeComponents)
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
            var compsData = GraphNodesContentsSerializer.Deserialize("ru-RU", id);
            ContentModel = compsData!;

            var comps = GraphComponentsFactory.ConstructComponents(compsData!.Components);
            NodeComponents.Clear();
            foreach (var comp in comps) NodeComponents.Add(comp);

            // setup all connectors data and id's
            DefineConnectors();
            IsConnectorsVisible = true;
        }

        public void ConstructNode(int nodeId)
        {
            string strId = nodeId.ToString();
            uint idFirstPart = uint.Parse(strId[0].ToString());

            GraphNodeType info = GraphNodesTypesSerializer.Deserialize("ru-RU", idFirstPart)!;
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
    }
}
