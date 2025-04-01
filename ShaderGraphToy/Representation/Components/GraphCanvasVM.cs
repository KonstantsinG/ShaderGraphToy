using ShaderGraphToy.Representation.GraphNodes;
using ShaderGraphToy.Utilities.DataBindings;
using ShaderGraphToy.Windows;

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

            foreach (var node in _nodes)
            {
                if (node.Selected)
                    corpses.Add(node);
            }

            foreach (var corp in corpses)
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
                placeNodeOnCanvas?.Invoke(node);
            }
        }
    }
}
