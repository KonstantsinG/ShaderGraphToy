using Nodes2Shader.Compilation.MathGraph;
using ShaderGraphToy.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShaderGraphToy.Representation.GraphNodes
{
    /// <summary>
    /// Логика взаимодействия для GraphNodeBase.xaml
    /// </summary>
    public partial class GraphNodeBase : UserControl
    {
        private static readonly BitmapImage _minusImg = ResourceManager.GetIconFromResources("minus_icon.png");
        private static readonly BitmapImage _arrowDownImg = ResourceManager.GetIconFromResources("arrowDown_icon.png");

        public bool Selected { get; private set; }
        public bool IsMinimized { get; private set; } = false;
        public int NodeId { get => ((GraphNodeBaseVM)DataContext).NodeId; }
        public uint NodeTypeId { get => ((GraphNodeBaseVM)DataContext).NodeModel!.Id; }

        public delegate void NodeStateHandler(GraphNodeBase sender);

        public event MouseButtonEventHandler HeaderPressed = delegate { };
        public event MouseEventHandler ConnectorPressed = delegate { };
        public event NodeStateHandler NodeStateChanged = delegate { };
        public event NodeStateHandler NodeSizeChanged = delegate { };


        public GraphNodeBase()
        {
            InitializeComponent();

            inputConnector.IsInput = true;
            outputConnector.IsInput = false;

            GraphNodeBaseVM vm = new();
            DataContext = vm;
            vm.GetOwnConnectors = GetConnectors;
            vm.RaiseConnectorPressedEvent = RaiseConnectorPressedEvent;
            vm.RaiseNodeSizeChangedEvent = RaiseNodeSizeChangedEvent;
            vm.HideOperationsCBoxes = HideOperationsCBoxes;
            operationsCBox.SelectionChanged += vm.OperationsComboBox_SelectionChanged;
            subOperationsCBox.SelectionChanged += vm.SubOperationsComboBox_SelectionChanged;

            operationsCBox.SelectionChanged += RaiseNodeStateChangedEvent;
            subOperationsCBox.SelectionChanged += RaiseNodeStateChangedEvent;
        }


        public bool ContainsConnector(NodesConnector? nc)
        {
            if (nc == null) return false;

            foreach (NodesConnector nc2 in ((GraphNodeBaseVM)DataContext!).Connectors)
            {
                if (nc2 == nc) return true;
            }

            return false;
        }

        public Rect GetBoundsRect() => new(((TranslateTransform)RenderTransform).X, ((TranslateTransform)RenderTransform).Y, RenderSize.Width, RenderSize.Height);


        public void ToggleSelection(bool isSelected)
        {
            Selected = isSelected;

            if (isSelected)
            {
                borderRect.Fill = (Brush)FindResource("SelectedBlue");
                borderRect.Margin = new Thickness(-3);
            }
            else
            {
                borderRect.Fill = (Brush)FindResource("Gray_00");
                borderRect.Margin = new Thickness(-1);
            }
        }

        private void HeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HeaderPressed.Invoke(this, e);
        }



        public List<NodesConnector> GetConnectors()
        {
            List<NodesConnector> conns = [];

            if (inputConnector.Visibility == Visibility.Visible) conns.Add(inputConnector);
            if (outputConnector.Visibility == Visibility.Visible) conns.Add(outputConnector);

            return conns;
        }

        public List<NodesConnector> GetInputs() => ((GraphNodeBaseVM)DataContext).Inputs;

        public NodeData GetNodeData() => ((GraphNodeBaseVM) DataContext).GetNodeData();



        public void RaiseConnectorPressedEvent(object sender, MouseEventArgs e)
        {
            ConnectorPressed.Invoke(sender, e);
        }

        private void RaiseNodeStateChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            NodeStateChanged.Invoke(this);
        }

        private void RaiseNodeSizeChangedEvent()
        {
            UpdateLayout();
            NodeSizeChanged.Invoke(this);
        }

        private void MinimizeImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMinimized)
            {
                operationsPanel.Visibility = Visibility.Visible;
                minimizeImg.Source = _minusImg;
            }
            else
            {
                operationsPanel.Visibility = Visibility.Collapsed;
                minimizeImg.Source = _arrowDownImg;
            }

            IsMinimized = !IsMinimized;

            UpdateLayout();
            NodeSizeChanged.Invoke(this);
        }

        public void HideOperationsCBoxes()
        {
            operationsPanel.Visibility = Visibility.Collapsed;
            minimizeImg.Source = _arrowDownImg;
            IsMinimized = true;

            UpdateLayout();
            NodeSizeChanged.Invoke(this);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Selected) borderRect.Fill = (Brush)FindResource("HighlightBlue");
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Selected) borderRect.Fill = (Brush)FindResource("Gray_00");
        }
    }
}
